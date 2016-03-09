var React = require('react')

var Landing = React.createClass({
  getInitialState: function() {
    return { keys: { total: '...', types: [] } }
  },
  componentDidMount: function() {
    this.serverRequest = $.get('/keys/counts', function(data) {
      var total = 0;
      for(var i = 0; i < data.counts.length; i++) {
        total = total + data.counts[i].count;
      }

      this.setState({ keys: { total: total, types: data.counts } })
    }.bind(this));
  },
  componentWillUnmount: function() {
    this.serverRequest.abort()
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
    for(var i = 0; i < this.state.keys.types.length; i++) {
      if(this.state.keys.types[i].key_type == type) {
        return this.state.keys.types[i].count.toLocaleString()
      }
    }

    return 0
  }
})

module.exports = Landing
