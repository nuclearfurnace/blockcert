var React = require('react')
var Link = require('react-router').Link

var Application = React.createClass({
  render: function() {
    return (
      <div id="content">
        <div className="spaced">
          <div className="top-bar search-bar">
            <div className="top-bar-left">
              <ul className="dropdown menu">
                <li className="menu-text">Blockcert</li>
                <li><Link to='/app/keys/create'>Create Key</Link></li>
                <li><Link to='/app/certifications/generate'>Certify Learner</Link></li>
                <li><Link to='/app/certifications/check'>Check Certifications</Link></li>
              </ul>
            </div>
          </div>
        </div>

        {this.props.children}
      </div>
    )
  }
})

module.exports = Application
