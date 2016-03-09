var React = require('react')
var Modal = require('react-modal')
var Select = require('react-select')
require('react-select/dist/react-select.css')

var CertifyLearner = React.createClass({
  getDefaultProps: function() {
    return { certifyModalHeight: 400, certifyModalWidth: 500 }
  },
  getInitialState: function() {
    return { selectValue: {}, certifyResult: {}, certifyModalShowing: false, submitDisabled: false }
  },
  render: function() {
    return (
      <div>
        <div className="row spaced">
          <div className="small-12 medium-8 medium-offset-2 columns">
            <h3>Certify A Learner</h3>
            <p>Generate a certification on the blockchain for a learner that has passed a course.</p>
          </div>
        </div>
        <div className="row">
          <div className="small-12 medium-8 medium-offset-2 columns">
            <form>
              {this.getFormErrors()}
              <div className="input-controls spaced">
                <label>Provider
                  <Select.Async
                    name="providerAddress"
                    autoload={false}
                    loadOptions={this.loadSelect('provider')}
                    value={this.state.selectValue['provider']}
                    onChange={this.handleSelectChange('provider')} />
                </label>
                <p className="help-text select-help-text" id="providerAddress">The provider which the learner took the course on.</p>
                <label>Organization
                  <Select.Async
                    name="organizationAddress"
                    autoload={false}
                    loadOptions={this.loadSelect('organization')}
                    value={this.state.selectValue['organization']}
                    onChange={this.handleSelectChange('organization')} />
                </label>
                <p className="help-text select-help-text" id="organizationAddress">The organization offering the course.</p>
                <label>Course
                  <Select.Async
                    name="courseAddress"
                    autoload={false}
                    loadOptions={this.loadSelect('course')}
                    value={this.state.selectValue['course']}
                    onChange={this.handleSelectChange('course')} />
                </label>
                <p className="help-text select-help-text" id="courseAddress">The course that was completed.</p>
                <label>Learner
                  <Select.Async
                    name="learnerAddress"
                    autoload={false}
                    loadOptions={this.loadSelect('learner')}
                    value={this.state.selectValue['learner']}
                    onChange={this.handleSelectChange('learner')} />
                </label>
                <p className="help-text select-help-text" id="learnerAddress">The learner that completed the course.</p>
              </div>
              <button type="button" className={this.getSubmitButtonClasses()} onClick={this.certifyLearner}>Certify</button>
            </form>
          </div>
        </div>

        <Modal isOpen={this.state.certifyModalShowing} onRequestClose={this.closeCertifyModal} style={this.generateModalStyling()}>
          <div className="text-center softer-text hard-callout">
            <h4>Certification successfully generated.</h4>
            <p>We sent a certification for to the given learner address:</p>
          </div>
          <div className="callout info softer-text text-center">
            <span>{this.state.certifyResult.learnerAddress}</span>
          </div>
          <div className="text-center softer-text hard-callout">
            <p>and we generated the following metadata hash:</p>
          </div>
          <div className="callout info softer-text text-center">
            <span>{this.state.certifyResult.metadataHash}</span>
          </div>
        </Modal>
      </div>
    )
  },
  generateModalStyling: function() {
    return {
      content: {
        width: this.props.certifyModalWidth,
        top: '150px',
        left: '50%',
        right: 'initial',
        bottom: 'initial',
        marginLeft: '-' + (this.props.certifyModalWidth / 2) + 'px'
      }
    }
  },
  closeCertifyModal: function() {
    this.setState({ certifyModalShowing: false })
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
          <h5>Woops! We hit an error certifying the learner!</h5>
          <ul className="no-bullet">
          {humanReadableErrors}
          </ul>
          </div>
      )
    }

    return null
  },
  getSubmitButtonClasses: function() {
    if(this.state.submitDisabled) {
      return "button expanded disabled"
    }

    return "button expanded"
  },
  handleSelectChange: function(type) {
    return function(value) {
      var existingSelectValue = this.state.selectValue
      existingSelectValue[type] = value

      this.setState({ selectValue: existingSelectValue })
    }.bind(this)
  },
  loadSelect: function(keyType) {
    return function(input, callback) {
      if(input == '') {
        callback(null, { options: [] })
        return
      }

      var encodedInput = encodeURIComponent(input)
      $.get('/keys/search/' + keyType + '/' + encodedInput, function(data) {
        var options = []
        for(var i = 0; i < data.results.length; i++) {
          options.push({ value: data.results[i].key_address, label: data.results[i].key_name + ' (' + data.results[i].key_address + ')' })
        }

        callback(null, { options: options })
      })
    }
  },
  certifyLearner: function() {
    // Clear our form errors and disable submit.
    this.setState({ formErrors: null, submitDisabled: true })

    var providerAddress = this.state.selectValue['provider'].value
    var organizationAddress = this.state.selectValue['organization'].value
    var courseAddress = this.state.selectValue['course'].value
    var learnerAddress = this.state.selectValue['learner'].value

    this.serverRequest = $.ajax({
      url: '/certify/execute',
      type: 'POST',
      data: JSON.stringify({
        providerAddress: providerAddress,
        organizationAddress: organizationAddress,
        courseAddress: courseAddress,
        learnerAddress: learnerAddress
      }),
      dataType: 'json',
      contentType: 'application/json; charset=utf-8',
      success: function(data) {
        this.setState({ certifyResult: data.result, certifyModalShowing: true })
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

module.exports = CertifyLearner
