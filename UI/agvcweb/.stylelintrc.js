module.exports = {
  plugins: ["stylelint-scss"],
  // extends: ["stylelint-config-standard"],
  rules: {
    "scss/dollar-variable-pattern": "^foo",
    "scss/selector-no-redundant-nesting-selector": true,
    "color-no-invalid-hex": true,
    "rule-empty-line-before": null,
    "color-hex-length": "long",
    "color-hex-case": "lower",
    "unit-whitelist": ["em", "rem", "%", "s", "px", "deg", "vh", "vw"],
    "declaration-colon-newline-after": null,
    "property-no-unknown": [
      true,
      {
        ignoreProperties: ["composes"]
      }
    ],
    "selector-pseudo-class-no-unknown": [
      true,
      {
        ignorePseudoClasses: ["global"]
      }
    ],
    // at-rule-no-unknown: 屏蔽一些scss等语法检查
    "at-rule-no-unknown": [true, {
      "ignoreAtRules": [
        "mixin", "extend", "content"
      ]
    }]
  }
};
