module.exports = {
  outputDir: "../wwwroot/",
  transpileDependencies: ["vuetify"],

  pluginOptions: {
    i18n: {
      locale: "es",
      fallbackLocale: "en",
      localeDir: "locales",
      enableInSFC: true
    }
  }
};
