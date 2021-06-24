const plugins = [
  [
    "component",
    {
      libraryName: "element-ui",
      styleLibraryName: "theme-chalk"
    },
    "syntax-dynamic-import"
  ]
];
// vue-cli -- build时自动清除console
if (process.env.NODE_ENV === "production") {
  plugins.push("transform-remove-console");
}
module.exports = {
  "compact": false,
  presets: [
    [
      "@vue/app",
      {
        useBuiltIns: "entry",
      }
    ]

  ],
  plugins
};
