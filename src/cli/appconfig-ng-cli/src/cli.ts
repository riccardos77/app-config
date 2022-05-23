#! /usr/bin/env node
import { readFileSync, writeFileSync } from 'fs';
import { JSONSchema4 } from 'json-schema';
import { compile, Options } from 'json-schema-to-typescript';
import { Options as PrettierOptions } from 'prettier';
import request from 'request';

const configFilePath = process.cwd() + '/' + (process.argv[2] ?? '.appconfig-cli.json');
console.log('reading configuration from ' + configFilePath);

const config: CliSettings = JSON.parse(readFileSync(configFilePath).toString()) as CliSettings;

const json2tsOptions = Object.assign<{}, Partial<Options>, Partial<Options>>(
  {},
  { bannerComment: '', strictIndexSignatures: true },
  config.json2ts
);

if (config.json2ts.styleFile) {
  json2tsOptions.style = JSON.parse(readFileSync(config.json2ts.styleFile).toString()) as PrettierOptions;
}

async function compileSchemaAsync(url: string): Promise<string> {
  return new Promise((resolve, reject) => {
    request({ uri: url }, function (error: unknown, response: unknown, body: string) {
      if (error !== undefined && error !== null) {
        process.stderr.write('Error downloading schema from ' + url);
        if (typeof error === 'string') {
          process.stderr.write(error);
        }

        reject(error);
        return;
      }

      compile(JSON.parse(body) as JSONSchema4, '', json2tsOptions)
        .then(code => {
          resolve(code);
          return;
        })
        .catch((err: unknown) => {
          process.stderr.write('Error generating types from schema');
          if (typeof err === 'string') {
            process.stderr.write(err);
          }

          reject(err);
          return;
        });
    });
  });
}

let allCode = '';

void compileSchemaAsync(
  `${config.origin.configInstanceUrl}apps/${config.origin.appName}/values/instances/${config.origin.appIdentity}/schema`
).then(r => {
  allCode += r;

  void compileSchemaAsync(
    `${config.origin.configInstanceUrl}apps/${config.origin.appName}/values/instances/${config.origin.appIdentity}/schema/tags`
  ).then(r2 => {
    allCode += '\n' + r2;
    writeFileSync(config.destination, allCode, { encoding: 'utf-8' });
  });
});

interface CliSettings {
  origin: ConfigOrigin;
  destination: string;
  json2ts: Options & { styleFile?: string };
}

interface ConfigOrigin {
  configInstanceUrl: string;
  appName: string;
  appIdentity: string;
}
