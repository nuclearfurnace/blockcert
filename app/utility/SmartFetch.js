var SmartFetch = function(url, options) {
  // If mode is set, and is 'json', set the headers and body to be JSON.
  if(options && options.mode && options.mode == 'json')
  {
    options.headers = {
      'Accept': 'application/json',
      'Content-Type': 'application/json'
    }

    options.body = JSON.stringify(options.body)
  }

  return fetch(url, options)
    .then(function(response) {
      // We always expect to get back JSON.
      return response.json()
    })
    .then(function(decoded) {
      // If we get a 'fail' or 'error', there's always going to be some sort
      // of message or error data associated with it, so just wrap that in
      // an error and throw it so the caller can catch.
      if(decoded.status == 'fail' || decoded.status == 'error') {
        var errorValue = decoded.message || decoded.data || 'an error occurred'
        var fetchError = new Error(errorValue)
        fetchError.response = response

        throw fetchError
      }

      // If there's no error/fail, then just unwrap the data.
      return decoded.data
    })
}

module.exports = SmartFetch
