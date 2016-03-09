var React = require('react')
var Select = require('react-select')
var moment = require('moment')
require('react-select/dist/react-select.css')

var CheckCertifications = React.createClass({
  getDefaultProps: function() {
    return { certified_dt_format: 'dddd, MMMM Do YYYY, h:mm:ss a' }
  },
  getInitialState: function() {
    return { selectValue: {}, certificationResults: [], hasSubmitted: false, submitDisabled: false }
  },
  render: function() {
    return (
      <div>
        <div className="row spaced">
          <div className="small-12 medium-8 medium-offset-2 columns">
            <h3>Check A Learner's Certifications</h3>
            <p>Examine the blockchain to see what courses a learner has passed.</p>
          </div>
        </div>
        <div className="row spaced">
          <div className="small-12 medium-8 medium-offset-2 columns">
            <form>
              {this.getFormErrors()}
              <div className="input-controls spaced">
                <label>Learner
                  <Select.Async
                    name="learnerAddress"
                    autoload={false}
                    loadOptions={this.loadSelect('learner')}
                    value={this.state.selectValue['learner']}
                    onChange={this.handleSelectChange('learner')} />
                </label>
                <p className="help-text select-help-text" id="learnerAddress">The learner to check.</p>
              </div>
              <button type="button" className={this.getSubmitButtonClasses()} onClick={this.checkCertifications}>Check</button>
            </form>
          </div>
        </div>
        {this.getCertificationItems()}
      </div>
    )
  },
  getCertificationItems: function() {
    if(this.state.hasSubmitted) {
      if(this.state.certificationResults.length == 0) {
        return (
          <div className="row text-center">
            <h3>Sorry.  No certifications to show.</h3>
          </div>
        )
      } else {
        return (
          <div className="row">
            <div className="small-12 columns">
              <h4>We found some results!</h4>
            </div>
            {this.state.certificationResults.map(this.mapCertificationResult)}
          </div>
        )
      }
    }
  },
  mapCertificationResult: function(rawCertification) {
    return (
      <div className="small-12 columns">
        <div className="callout">
          <h5>{rawCertification.course_name}</h5>
          <p>Served via <strong>{rawCertification.provider_name}</strong> and created by <strong>{rawCertification.organization_name}</strong>.  Certified on <strong>{moment.unix(rawCertification.certified_at/1000).format(this.props.certified_dt_format)}</strong>.</p>
          <p><a href={'https://btc.blockr.io/tx/info/' + rawCertification.certification_txid}>Link</a> to certification transaction.</p>
        </div>
      </div>
    )
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
          <h5>Woops! We hit an error looking for learner certifications!</h5>
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
  checkCertifications: function() {
    // Clear our form errors and disable submit.
    this.setState({ formErrors: null, submitDisabled: true })

    var learnerAddress = this.state.selectValue['learner'].value

    this.serverRequest = $.ajax({
      url: '/certify/check/' + learnerAddress,
      dataType: 'json',
      contentType: 'application/json; charset=utf-8',
      success: function(data) {
        this.setState({ certificationResults: data.results })
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
        this.setState({ hasSubmitted: true, submitDisabled: false })
      }.bind(this)
    })
  }
})

module.exports = CheckCertifications
