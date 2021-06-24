module.exports = {
  root: true,
  env: {
    node: true,
    commonjs: true,
    "browser": true,
    es6: true
  },
  extends: ["eslint:recommended", "plugin:vue/essential"],
  globals: {
    Atomics: "readonly",
    SharedArrayBuffer: "readonly",
    proccess: true
  },
  parserOptions: {
    "parser": "babel-eslint",
    ecmaVersion: 2018,
    sourceType: "module",
    "ecmaFeatures": {
      "jsx": true,
      "modules": true,
      "experimentalObjectRestSpread": true
    }
  },
  plugins: ["vue"],
  rules: {}
};
