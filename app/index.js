var domready = require('domready')
var React = require('react')
var ReactDOM = require('react-dom')
var ReactRouter = require('react-router')
var Router = ReactRouter.Router
var Route = ReactRouter.Route
var IndexRoute = ReactRouter.IndexRoute
var browserHistory = ReactRouter.browserHistory
var Application = require('components/Application')
var Landing = require('components/Landing')
var CreateKey = require('components/CreateKey')
var CertifyLearner = require('components/CertifyLearner')
var CheckCertifications = require('components/CheckCertifications')

domready(() => {
  ReactDOM.render((
    <Router history={browserHistory}>
      <Route path="/" component={Application}>
        <IndexRoute component={Landing} />
        <Route path="/app/keys/create" component={CreateKey} />
        <Route path="/app/certifications/generate" component={CertifyLearner} />
        <Route path="/app/certifications/check" component={CheckCertifications} />
      </Route>
    </Router>
  ), document.getElementById('application'))
})
