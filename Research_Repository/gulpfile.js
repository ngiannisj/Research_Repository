/// <binding AfterBuild='default' Clean='clean' />
const gulp = require('gulp');
const uglify = require('gulp-uglify');
const concat = require('gulp-concat');
const cleanCSS = require('gulp-clean-css');
const autoprefixer = require('gulp-autoprefixer');
const rename = require('gulp-rename');

//Constants
const sassAction = "build-sass";
const jsAction = "build-js";

let filePaths = {
    sassInputPath: "./wwwroot/css/styles.min.css",
    sassOutputPath: "./wwwroot/css",
    jsInputPath: "./Content/js/**/*.js",
    jsOutputPath: "./wwwroot/js"
};

//CSS action
gulp.task(sassAction, () => {
    return gulp.src(filePaths.sassInputPath)
        .pipe(autoprefixer({
            cascade: false
        }))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(rename({
            basename: "styles",
            suffix: ".min",
            extname: ".css"
        }))
        .pipe(gulp.dest(filePaths.sassOutputPath));
});

//JS action
gulp.task(jsAction, () => {
    return gulp.src(filePaths.jsInputPath )
        .pipe(concat('scripts.js'))
        .pipe(uglify())
        .pipe(rename({
            basename: "scripts",
            suffix: ".min",
            extname: ".js"
        }))
        .pipe(gulp.dest(filePaths.jsOutputPath))
});

//Combine actions so only one action needs to be executed
exports.default = gulp.series([sassAction, jsAction]);