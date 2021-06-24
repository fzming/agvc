"use strict";
const path = require("path");
const webpack = require("webpack");

function resolve(dir) {
  return path.join(__dirname, dir);
}

// If your port is set to 80,
// use administrator privileges to execute the command line.
// For example, Mac: sudo npm run
const port = 9898; // dev port

// All configuration item explanations can be find in https://cli.vuejs.org/config/
module.exports = {
  /**
   * You will need to set publicPath if you plan to deploy your site under a sub path,
   * for example GitHub Pages. If you plan to deploy your site to https://foo.github.io/bar/,
   * then publicPath should be set to "/bar/".
   * In most cases please use '/' !!!
   * Detail: https://cli.vuejs.org/config/#publicpath
   */
  //publicPath: '/',
  outputDir: "dist",
  assetsDir: "static",
  lintOnSave: process.env.NODE_ENV === "development",
  productionSourceMap: false,
  // 是否使用包含运行时编译器的 Vue 构建版本
  runtimeCompiler: true,
  devServer: {
    open: process.platform === "darwin", // 配置自动启动浏览器
    host: "0.0.0.0",
    port: port,
    clientLogLevel: "info",
    hotOnly: false,
    proxy: null, // 设置代理
    disableHostCheck: true, // 禁用域名检查
    before: (app) => {
      app !== undefined;
    },
  },
  configureWebpack: (config) => {
    if (process.env.NODE_ENV === "production") {
      // 启用CDN加速,把那些不太可能改动的代码或者库分离出来
      config.externals = {
        vue: "Vue",
        "vue-router": "VueRouter",
        vuex: "Vuex",
        jquery: "jQuery",
        lodash: "_",
        axios: "axios",
      };
    }
  },
  chainWebpack(config) {
    config.plugins.delete("preload"); // TODO: need test
    config.plugins.delete("prefetch"); // TODO: need test
    // 配置中启用 node.__filename
    config.node
      .set("__dirname", true) // 同理
      .set("__filename", true);
    if (process.env.NODE_ENV === "development") {
      // 修复HMR 热更新失效的BUG
      config.resolve.symlinks(true);
    }
    //修复 Lazy loading routes Error
    config.plugin("html").tap((args) => {
      args[0].chunksSortMode = "none";
      return args;
    });
    config.resolve.alias
      // https://segmentfault.com/a/1190000006435886
      .set("vue$", "vue/dist/vue.esm.js") // 默认是vue.runtime.esm.js  //不要采用runtime形式的文件,而采用 dist/vue.esm.js形式文件，原因是runtime模式不包含编译器
      .set("@", resolve("src")) // key,value自行定义，比如.set('@@', resolve('src/components'))
      .set("@assets", resolve("src/assets"))
      .set("vendor", resolve("/src/vendor"));
    // 引入jquery支持========================================
    config.plugin("provide").use(webpack.ProvidePlugin, [
      {
        $: "jquery",
        jquery: "jquery",
        jQuery: "jquery",
        "window.jQuery": "jquery",
      },
    ]);
    // set svg-sprite-loader
    config.module.rule("svg").exclude.add(resolve("src/icons")).end();
    config.module
      .rule("icons")
      .test(/\.svg$/)
      .include.add(resolve("src/icons"))
      .end()
      .use("svg-sprite-loader")
      .loader("svg-sprite-loader")
      .options({
        symbolId: "icon-[name]",
      })
      .end();
    //yaml-loader
    config.resolve.extensions.add(".yml").add(".yaml");
    config.module
      .rule("yaml")
      .test(/\.ya?ml?$/)
      .use("json-loader")
      .loader("json-loader")
      .end()
      .use("yaml-loader")
      .loader("yaml-loader")
      .end();

    // set preserveWhitespace
    // config.module
    //   .rule('vue')
    //   .use('vue-loader')
    //   .loader('vue-loader')
    //   .tap(options => {
    //     options.compilerOptions.preserveWhitespace = true;
    //     return options;
    //   })
    //   .end();

    config
      // https://webpack.js.org/configuration/devtool/#development
      .when(process.env.NODE_ENV === "development", (config) =>
        config.devtool("cheap-source-map")
      );

    config.when(process.env.NODE_ENV !== "development", (config) => {
      config
        .plugin("ScriptExtHtmlWebpackPlugin")
        .after("html")
        .use("script-ext-html-webpack-plugin", [
          {
            // `runtime` must same as runtimeChunk name. default is `runtime`
            inline: /runtime\..*\.js$/,
          },
        ])
        .end();
      config.optimization.splitChunks({
        chunks: "all",
        cacheGroups: {
          libs: {
            name: "chunk-libs",
            test: /[\\/]node_modules[\\/]/,
            priority: 10,
            chunks: "initial", // only package third parties that are initially dependent
          },
          elementUI: {
            name: "chunk-elementUI", // split elementUI into a single package
            priority: 20, // the weight needs to be larger than libs and app or it will be packaged into libs or app
            test: /[\\/]node_modules[\\/]_?element-ui(.*)/, // in order to adapt to cnpm
          },
          commons: {
            name: "chunk-components",
            test: resolve("src/components"), // can customize your rules
            minChunks: 3, //  minimum common number
            priority: 5,
            reuseExistingChunk: true,
          },
        },
      });
      config.optimization.runtimeChunk("single");
    });
  },
  css: {
    //css预设器配置项 详见https: //cli.vuejs.org/zh/config/#css-loaderoptions
    loaderOptions: {
      // 给 sass-loader 传递选项
      sass: {
        // data: fs.readFileSync('src/styles/variables.scss', 'utf-8')
        prependData: `@import "~@/styles/variables.scss";`,
      },
    },
    // 启用 CSS modules        //在VUE CLI 4 中已经被 放弃使用
    //在VUE CLI 4 中 css.requireModuleExtension 默认情况下，
    //只有 *.module.[ext] 结尾的文件才会被视作 CSS Modules 模块。
    //设置为 false 后你就可以去掉文件名中的 .module 并将所有的 *.(css|scss|sass|less|styl(us)?) 文件视为 CSS Modules 模块。
    // modules: false
  },
  // 进行编译的依赖
  transpileDependencies: ["element-ui"],
};
