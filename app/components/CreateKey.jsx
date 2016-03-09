var React = require('react')
var Modal = require('react-modal')
var QRCode = require('qrcode.react')

var CreateKey = React.createClass({
  getDefaultProps: function() {
    return { keyModalHeight: 400, keyModalWidth: 600 }
  },
  getInitialState: function() {
    return { keyModalShowing: false, submitDisabled: false }
  },
  render: function() {
    return (
      <div>
        <div className="row spaced">
          <div className="small-12 medium-8 medium-offset-2 columns">
            <h3>Create A New Key</h3>
            <p>A key is tied to an edX provider, an organization, or a course.  When keys from a provider, organization, and course are combined, they have the power to assert that a learner has passed a course.</p>
          </div>
        </div>
        <div className="row">
          <div className="small-12 medium-8 medium-offset-2 columns">
            <form>
              {this.getFormErrors()}
              <div className="input-controls spaced">
                <label>Key Name
                  <input type="text" ref="keyName" name="keyName" aria-describedby="keyNameHelpText" />
                </label>
                <p className="help-text" id="keyNameHelpText">This should represent the name of the the owner of the key.  For example, use the web address or name of the owner, such as <strong>edx.org</strong> or <strong>Massachusetts Institute Of Technology</strong>.</p>
                <label>Key Type
                  <select ref="keyType" name="keyType" defaultValue="provider" aria-describedby="keyTypeHelpText">
                    <option value="provider">Provider</option>
                    <option value="organization">Organization</option>
                    <option value="course">Course</option>
                    <option value="learner">Learner</option>
                  </select>
                </label>
                <p className="help-text" id="keyTypeHelpText">Select an appropriate type for the key.  If this is for an edX provider (the company or entity running the edX instance), choose <strong>Provider</strong>.  If this is for an organization actually providing course content, choose <strong>Organization</strong>.  If this is for a specific course, choose <strong>Course</strong>. If this is for a learner, choose <strong>Learner</strong>.</p>
              </div>
              <button type="button" className={this.getSubmitButtonClasses()} onClick={this.generateKey}>Generate</button>
            </form>
          </div>
        </div>

        <Modal isOpen={this.state.keyModalShowing} onRequestClose={this.closeKeyModal} style={this.generateModalStyling()}>
          <div className="text-center softer-text hard-callout">
            <h4>Key successfully generated.</h4>
            <p>Scan the QR code or copy the private key below.</p>
          </div>
          <div className="qrcode-container-256 spaced">
            <QRCode value={this.state.privateKey} size={256} />
          </div>
          <div className="callout success softer-text text-center">
            <span>{this.state.privateKey}</span>
          </div>
        </Modal>
      </div>
    )
  },
  getSubmitButtonClasses: function() {
    if(this.state.submitDisabled) {
      return "button expanded disabled"
    }

    return "button expanded"
  },
  generateModalStyling: function() {
    return {
      content: {
        width: this.props.keyModalWidth,
        top: '150px',
        left: '50%',
        right: 'initial',
        bottom: 'initial',
        marginLeft: '-' + (this.props.keyModalWidth / 2) + 'px'
      }
    }
  },
  closeKeyModal: function() {
    this.setState({ keyModalShowing: false })
  },
  getFormErrors: function() {
    if(this.state.formErrors) {
      var errors = this.state.formErrors.map(function(err) {
        return err.messages
      })

      var humanReadableErrors = [].concat.apply([], errors).map(function(err) {
        return <li>{err}</li>
      })

      return (
          <div className="callout alert">
          <h5>Woops! We hit an error generating your key!</h5>
          <ul className="no-bullet">
          {humanReadableErrors}
          </ul>
          </div>
      )
    }

    return null
  },
  generateKey: function() {
    // Clear any form errors.
    this.setState({ formErrors: null, submitDisabled: true })

    var keyName = this.refs.keyName.value
    var keyType = this.refs.keyType.value

    this.serverRequest = $.ajax({
      url: '/keys/create',
      type: 'POST',
      data: JSON.stringify({ keyName: keyName, keyType: keyType }),
      dataType: 'json',
      contentType: 'application/json; charset=utf-8',
      success: function(data) {
        this.setState({ privateKey: data.result.key_private, keyModalShowing: true })
      }.bind(this),
      error: function(xhr) {
        try {
          var data = $.parseJSON(xhr.responseText)
          if(data.errors) {
            this.setState({ formErrors: data.errors })
          }
        } catch(e) {
        }
      }.bind(this),
      complete: function() {
        this.setState({ submitDisabled: false })
      }.bind(this)
    })
  }
})

module.exports = CreateKey
