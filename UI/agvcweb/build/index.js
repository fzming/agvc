const { sh } = require('tasksfile')
const config = require('../vue.config.js')
const rawArgv = process.argv.slice(2)
const args = rawArgv.join(' ')

if (process.env.npm_config_preview || rawArgv.includes('--preview')) {
  const report = rawArgv.includes('--report')

  sh(`vue-cli-service build ${args}`)

  const port = 9526
  const publicPath = config.publicPath

  var connect = require('connect')
  var serveStatic = require('serve-static')
  const app = connect()

  app.use(
    publicPath,
    serveStatic('./dist', {
      index: ['index.html', '/']
    })
  )

  app.listen(port, function () {
    console.log(`> 预览： at  http://localhost:${port}${publicPath}`)
    if (report) {
      console.log(`> 生成报告： at  http://localhost:${port}${publicPath}report.html`)
    }
  })
} else {
  sh(`vue-cli-service build ${args}`)
}
