/// <binding AfterBuild='default' Clean='clean' />
let gulp = require('gulp');
let gulp_sass = require('gulp-sass')(require('sass'));
const concat = require('gulp-concat');
const uglify = require('gulp-uglify');
const cleanCSS = require('gulp-clean-css');
const autoprefixer = require('gulp-autoprefixer');
let rename = require('gulp-rename');

//Constants
const sassAction = "build-sass";
const jsAction = "build-js";

let filePaths = {
    sassInputPath: "./Content/sass/**/*.scss",
    sassOutputPath: "./wwwroot/css",
    jsInputPath: "./Content/js/**/*.js",
    jsOutputPath: "./wwwroot/js"
};

//CSS action
gulp.task(sassAction, () => {
    return gulp.src(filePaths.sassInputPath)
        .pipe(concat('styles.css'))
        .pipe(gulp_sass({
            style: 'compressed'
        }).on('error', gulp_sass.logError))
        .pipe(autoprefixer({
            cascade: false,
            grid: 'no-autoplace'
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