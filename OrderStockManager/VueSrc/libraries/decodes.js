export function decode_base64(base64string) {
  let missing_padding = base64string.length % 4
  if (missing_padding != 0) {
    base64string = base64string + Array(1 + 4 - missing_padding).join('=')
  }
  return new Buffer(base64string, 'base64').toString()
}

export function readJwtPayload(jwtToken) {
  var base64Url = jwtToken.split('.')[1]
  var jsonString = decode_base64(base64Url)
  return JSON.parse(jsonString)
}
