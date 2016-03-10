var React = require('react')
var SmartFetch = require('utility/SmartFetch')

var Landing = React.createClass({
  getInitialState: function() {
    return { keys: { total: '...', types: {} } }
  },
  componentDidMount: function() {
    SmartFetch('/api/keys/counts')
      .then(function(data) {
        this.setState({ keys: { total: data.total, types: data.counts } })
      }.bind(this));
  },
  componentWillUnmount: function() {
  },
  render: function() {
    return (
      <div className="row landing text-center softer-text">
        <div className="small-12 columns spaced">
          <h1>Blockcert At A Glance</h1>
        </div>
        <div className="small-12 medium-6 columns">
          <p className="big-stat-name">Providers registered</p>
          <div className="big-stat-value">{this.getKeyCount('provider')}</div>
        </div>
        <div className="small-12 medium-6 columns">
          <p className="big-stat-name">Organizations registered</p>
          <div className="big-stat-value">{this.getKeyCount('organization')}</div>
        </div>
        <div className="small-12 medium-6 columns">
          <p className="big-stat-name">Courses registered</p>
          <div className="big-stat-value">{this.getKeyCount('course')}</div>
        </div>
        <div className="small-12 medium-6 columns">
          <p className="big-stat-name">Learners registered</p>
          <div className="big-stat-value">{this.getKeyCount('learner')}</div>
        </div>
      </div>
    )
  },
  getKeyCount: function(type) {
    if(this.state.keys.types[type]) {
      return this.state.keys.types[type].toLocaleString()
    }

    return 0
  }
})

module.exports = Landing
