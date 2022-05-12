import { readFileSync, writeFileSync } from 'fs';
import { JSONSchema4 } from 'json-schema';
import { compile, Options } from 'json-schema-to-typescript';
import { Options as PrettierOptions } from 'prettier';
import request from 'request';

const config: CliSettings = JSON.parse(readFileSync(process.cwd() + '\\.appconfig-cli.json').toString()) as CliSettings;

const url = `${config.origin.configInstanceUrl}apps/${config.origin.appName}/values/instances/${config.origin.appIdentity}/schema`;
request({ uri: url }, function (error: unknown, response: unknown, body: string) {
  if (error !== undefined && error !== null) {
    process.stderr.write('Error downloading schema from ' + url);
    if (typeof error === 'string') {
      process.stderr.write(error);
    }
    return;
  }

  const json2tsOptions = Object.assign<unknown, Partial<Options>, Partial<Options>>(
    {},
    { bannerComment: '', strictIndexSignatures: true },
    config.json2ts
  );
  if (config.json2ts.styleFile) {
    json2tsOptions.style = JSON.parse(readFileSync(config.json2ts.styleFile).toString()) as PrettierOptions;
  }

  compile(JSON.parse(body) as JSONSchema4, '', json2tsOptions)
    .then(code => {
      writeFileSync(config.destination, code, { encoding: 'utf-8' });
    })
    .catch((err: unknown) => {
      process.stderr.write('Error generating types from schema');
      if (typeof err === 'string') {
        process.stderr.write(err);
      }
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
