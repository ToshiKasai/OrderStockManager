import replace from 'rollup-plugin-replace';
import alias from 'rollup-plugin-alias';
import vue from 'rollup-plugin-vue';
import nodeResolve from 'rollup-plugin-node-resolve';
import commonjs from 'rollup-plugin-commonjs';
import nodeGlobals from 'rollup-plugin-node-globals';
import buble from 'rollup-plugin-buble';
import uglify from 'rollup-plugin-uglify';
import postcss from 'rollup-plugin-postcss';

const outdir = "Scripts/";

export default {
    entry: 'VueSrc/app.js',
    dest: outdir + 'app.js',
    plugins: [
        replace({
            'process.env.NODE_ENV': JSON.stringify('development') // development or production
        }),
        alias({
            // 'vue': 'node_modules/vue/dist/vue.esm.js'
        }),
        vue({
            // css: true,
            // css: "./dist/bundle.css",
            compileTemplate: true,
            scss: {
                outputStyle: "compressed" // expanded / compressed
            },
            pug: {
                pretty: false
            }
        }),
        postcss(),
        nodeResolve({
            jsnext: true,
            browser: true
        }),
        commonjs(/*{
            namedExports: {
                'node_modules/js-cookie/src/js.cookie.js': ['Cookies']
            }
        }*/),
        nodeGlobals(),
        buble()
        // ,
        // uglify()
    ],
    format: 'iife'
};
