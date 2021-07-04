/// <binding AfterBuild='default' Clean='clean' />
let gulp = require('gulp');
let gulp_sass = require('gulp-sass')(require('sass'));
const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
let rename = require('gulp-rename');
var gulp_typescript = require('gulp-typescript');
let tsProject = gulp_typescript.createProject('tsconfig.json'); //Uses tsconfig.json file for ts file configuration

//Constants
const sassAction = "build-sass";
const tsAction = "build-ts";
const watchAction = "watch"

let filePaths = {
    sassInputPath: "./Content/sass/**/*.scss",
    sassOutputPath: "./wwwroot/css",
    tsInputPath: "./Content/ts/**/*.ts",
    tsOutputPath: "./wwwroot/js"
};

gulp.task(sassAction, () => {
    return gulp.src(filePaths.sassInputPath)
        .pipe(concat('styles.css'))
        .pipe(gulp_sass({
            style: 'compressed'
        }).on('error', gulp_sass.logError))
        .pipe(rename({
            basename: "styles",
            suffix: ".min",
            extname: ".css"
        }))
        .pipe(gulp.dest(filePaths.sassOutputPath));});

gulp.task(tsAction, () => {
    return gulp.src(filePaths.tsInputPath)
        .pipe(tsProject())
        .pipe(concat('scripts.js'))
        .pipe(uglify())
        .pipe(rename({
            basename: "scripts",
            suffix: ".min",
            extname: ".js"
        }))
        .pipe(gulp.dest(filePaths.tsOutputPath))});

gulp.task(watchAction, () => {
    gulp.watch(filePaths.sassInputPath, gulp.series(sassAction));
    gulp.watch(filePaths.tsInputPath, gulp.series(tsAction))
});

exports.default = gulp.series([sassAction, tsAction, watchAction]);