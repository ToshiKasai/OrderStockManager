"use strict";

const fs = require('fs');
const moment = require('moment');
const crypto = require("crypto");
const glob = require('glob');
const optimist = require('optimist');
const path = require('path');

var args = optimist.usage('特定フォルダ内の一覧をJSONファイルに出力\nUsage: $0')
    .boolean('h').alias('h', 'help').describe('h', 'ヘルプの表示')
    .string('s').alias('s', 'src').default('s', './').describe('s', '入力ディレクトリ')
    .string('f').alias('f', 'file').describe('f', '対象とするファイルを指定')
    .string('d').alias('d', 'dest').default('d', './listup.json').describe('d', '出力ファイル')
    .string('n').alias('n', 'name').describe('n', 'JSON定義名')
    .boolean('p').alias('p', 'pretty').describe('p', 'pretty出力')
    .argv;

if (args.help) {
    optimist.showHelp();
    return;
}

// ファイル名リスト(glob)
var srcglob = [];
if (args.file) {
    if (Array.isArray(args.file)) {
        srcglob = srcglob.concat(args.file);
    } else {
        srcglob.push(args.file);
    }
}
if (args._) {
    srcglob = srcglob.concat(args._);
}
if (srcglob.length == 0) {
    srcglob.push('*.*');
}

// 対象ファイルリスト
var srclist = [];
for (var src in srcglob) {
    let tmp = glob.sync(path.join(args.s, srcglob[src]));
    if (tmp.length >= 1) srclist = srclist.concat(tmp);
}
srclist = srclist.filter((x, i, self) => self.indexOf(x) === i);

let resultjson = {};
for (var index in srclist) {
    let basename = path.basename(srclist[index], path.extname(srclist[index]));
    let name = basename + path.extname(srclist[index]).replace('.', '');
    let mtime = getMtime(srclist[index]);
    let fmd5 = getMD5HashWithFile(srclist[index]);
    let mhash = getMD5Hash(mtime);
    resultjson[name] = {
        "file": path.relative(args.s, srclist[index]).replace(/\\/g, '/'),
        "mtime": mtime,
        "md5": fmd5,
        "timehash": mhash
    }
}

if (args.n) {
    let result = resultjson;
    resultjson = {};
    resultjson[args.n] = result;
} else {
}

if (args.p) {
    fs.writeFileSync(args.d, JSON.stringify(resultjson, null, 4));
} else {
    fs.writeFileSync(args.d, JSON.stringify(resultjson));
}


function getMtime(file) {
    var stat = fs.statSync(file);
    // return moment(stat.mtime).format('YYYY/MM/DD HH:mm:ss.SSS ZZ');
    return moment(stat.mtime).format('x');
    // return stat.size;
}

function getMD5Hash(plane) {
    var md5 = crypto.createHash('md5');
    md5.update(plane);
    return md5.digest('hex');
}

function getSha512Hash(plane) {
    var sha512 = crypto.createHash('sha512');
    sha512.update(plane);
    return sha512.digest('hex');
}

function getMD5HashWithFile(file) {
    var content = fs.readFileSync(file);
    var md5 = crypto.createHash('md5');
    md5.update(content);
    return md5.digest('hex');
}

// var filename = './Scripts/app.js'; //process.argv[2];
// var md5 = getMD5HashWithFile(filename);
