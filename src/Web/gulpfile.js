/// <binding BeforeBuild='build' Clean='clean' ProjectOpened='watch' />
var config = require("./gulp/gulp.config");

var del = require("del");
var runSequence = require("run-sequence");
var gulp = require("gulp");
var concat = require("gulp-concat");
var cssmin = require("gulp-cssmin");
var uglify = require("gulp-uglify");
var sourcemaps = require("gulp-sourcemaps");
var less = require("gulp-less");
var rename = require("gulp-rename");
var rewriteCSS = require("gulp-rewrite-css");
var autoprefixer = require("gulp-autoprefixer");
var order = require("gulp-order");
var mainBowerFiles = require("main-bower-files");
var replace = require("gulp-replace");
var rev = require("gulp-rev");
var inject = require('gulp-inject');

require("es6-promise").polyfill();
require("rimraf");

gulp.task("clean", function () {
    return del(config.clean);
});

gulp.task("scripts:libs", function () {
    var vendors = mainBowerFiles("**/*.js");
    var allLibs = vendors.concat(config.jsLibs.in);

    return gulp.src(allLibs)
        .pipe(concat(config.jsLibs.outFile))
        .pipe(uglify())
        .pipe(gulp.dest(config.dest));
});

gulp.task("css:libs", function () {
    var vendors = mainBowerFiles("**/*.css");
    var allLibs = vendors.concat(config.cssLibs.in);

    return gulp.src(allLibs)
        .pipe(rewriteCSS({ destination: config.dest }))
        .pipe(cssmin())
        .pipe(replace("../../wwwroot/lib/summernote/dist/font/", "/content/fonts/"))
        .pipe(concat(config.cssLibs.outFile))
        .pipe(gulp.dest(config.dest));
});

gulp.task("libs", function () {
    runSequence(["scripts:libs", "css:libs"]);
});

gulp.task("scripts", function () {
    return gulp.src(config.js.in)
        .pipe(sourcemaps.init())
        .pipe(concat(config.js.outFile))
        .pipe(uglify())
        .pipe(rev())
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(config.dest));
});

gulp.task("scripts:prod", function () {
    return gulp.src(config.js.in)
        .pipe(concat(config.js.outFile))
        .pipe(uglify())
        .pipe(gulp.dest(config.dest));
});

gulp.task("less", function () {
    return gulp.src(config.less.in)
        .pipe(sourcemaps.init())
        .pipe(less())
        .pipe(cssmin())
        .pipe(concat(config.less.outFile))
        .pipe(autoprefixer({
            browsers: ["last 2 versions"],
            cascade: false
        }))
        .pipe(sourcemaps.write())
        .pipe(gulp.dest(config.dest));
});

gulp.task("build", function () {
    runSequence("clean", ["libs", "scripts", "less"], "inject");
});

gulp.task("build:prod", function () {
    runSequence("clean", ["libs", "scripts:prod", "less"], "inject");
});

gulp.task("watch", function () {
    gulp.watch([config.less.watch], ["less"]);
    gulp.watch([config.js.watch], ["scripts"]);
});

gulp.task("inject", function() {
    var target = gulp.src('Index.html');
    var sources = gulp.src([config.dest + "*.js"], { read: false });

    return target.pipe(inject(sources))
        .pipe(gulp.dest(''));
});