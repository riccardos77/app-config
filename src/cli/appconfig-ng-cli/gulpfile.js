'use strict';

var gulp = require('gulp');
var del = require('del');
var execSync = require('child_process').exec;
var dist = './dist';
var source = '.';

gulp.task('clean', function () {
  return del([dist]);
});

gulp.task('tsc', function (cb) {
  execSync('tsc --project ' + source, function (err, stdout, stderr) {
    if (stdout) {
      console.log(stdout);
    }
    if (stderr) {
      console.log(stderr);
    }
    cb(err);
  });
});

gulp.task('copy-package-json', function () {
  return gulp.src([source + '/package.json']).pipe(gulp.dest(dist));
});

gulp.task('pack', function (cb) {
  execSync('cd ' + dist + ' && npm pack', function (err, stdout, stderr) {
    if (stdout) {
      console.log(stdout);
    }
    if (stderr) {
      console.log(stderr);
    }
    cb(err);
  });
});

gulp.task(
  'build',
  gulp.series(
    'clean',
    'tsc',
    'copy-package-json',
    'pack'
  )
);
