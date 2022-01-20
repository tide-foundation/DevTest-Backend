# Backend Developer Test

Create a RESTful API that will function as a repository for cryptographic private keys.

The only type of secret keys that the system will handle are those of the form *prime256v1* Elliptic Curve and the format in which the system expects the serialized key is in the PEM format.

### Example of a *prime256v1* private key in PEM format

```bash
openssl ecparam -name prime256v1 -genkey -noout -out private.pem
cat private.pem
-----BEGIN EC PRIVATE KEY-----
MHcCAQEEINOO78gGFlUz8jt7UNUZNG+Fbq2XhuLl67Lktitx/sX1oAoGCCqGSM49
AwEHoUQDQgAESTypEXFKHFjfLoyuJ+NEml+POYGKfBJJXQOEFTP9hRZfcR9TZP7y
KHaBrnM2qc65F5JSLuWGAjhe6gAnLdcTZw==
-----END EC PRIVATE KEY-----
```

The Web API will contain two endpoints:
* `/key` where the private keys will be inserted, updated, deleted and obtained.
* `/signature` where the private keys will be used to generate a digital signature.
---
## `/key` endpoint
The endpoint will accept Json objects of the following form:
```json
{
  "id":1,
  "key":"MHcCAQEEINOO78gGFlUz8jt7UNUZNG+Fbq2XhuLl67Lktitx/sX1oAoGCCqGSM49AwEHoUQDQgAESTypEXFKHFjfLoyuJ+NEml+POYGKfBJJXQOEFTP9hRZfcR9TZP7yKHaBrnM2qc65F5JSLuWGAjhe6gAnLdcTZw=="
}
```
> Note that the property key value is identical to the private key example, except for two differences:
> 1. The text at the top and the bottom was removed.
> 2. The base64 encoded key has no spaces or new lines.

### List of all actions for the `/key` endpoint

| API                  | Description             | Request body | Response body |
| ------------------   | ------------------      | ------------ | ------------- |
| GET `/key`           | Get all the key items   | None         | List of keys |
| GET `/key`/*{id}*    | Get an item by id       | None         | key item |
| POST `/key`          | Add a new item          | key item     | key item |
| PUT `/key`/*{id}*    | Update an existing item | key item     | None |
| DELETE `/key`/*{id}* | Delete an item          | None         | None |

### Request example

```http
POST /key HTTP/1.1
Content-Type: application/json

{
  "key":"MHcCAQEEINOO78gGFlUz8jt7UNUZNG+Fbq2XhuLl67Lktitx/sX1oAoGCCqGSM49AwEHoUQDQgAESTypEXFKHFjfLoyuJ+NEml+POYGKfBJJXQOEFTP9hRZfcR9TZP7yKHaBrnM2qc65F5JSLuWGAjhe6gAnLdcTZw=="
}
```

### Response example

```
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id":1,
  "key":"MHcCAQEEINOO78gGFlUz8jt7UNUZNG+Fbq2XhuLl67Lktitx/sX1oAoGCCqGSM49AwEHoUQDQgAESTypEXFKHFjfLoyuJ+NEml+POYGKfBJJXQOEFTP9hRZfcR9TZP7yKHaBrnM2qc65F5JSLuWGAjhe6gAnLdcTZw=="
}
```

---

## `/signature` endpoint

The endpoint will look up the requested *prime256v1* private key and the received message to perform an *ECDSA* *SHA3-256* signature.

### Example of a signature

```bash
printf "Hello World" | openssl dgst -sha3-256 -sign private.pem | base64 -w 0
MEQCICnAqHQgNiEBEt58fewUVw1yzgT5yr86RCN1BD2siYaLAiB6Z0yQfZ9Hflk5G5WIcjBi8atO925hC1p23QoD5lvq6w==
```

> In this case the `Hello World` message was signed with the previously generated private key using the *SHA3-256* hash function and then *base64* encoded.

### Endpoint definition

| API | Description | Request body | Response body |
| --- | --- | --- | --- |
| GET `/signature`?keyId=*{keyId}*&message=*{base64url(message)}* | Sign the message with the key | None | *Base64* encoded signature |

### Request example

```http
GET /signature?keyId=1&message=SGVsbG8gV29ybGQ HTTP/1.1
```

### Response example

```
HTTP/1.1 200 OK
Content-Type: text/plain

MEQCICnAqHQgNiEBEt58fewUVw1yzgT5yr86RCN1BD2siYaLAiB6Z0yQfZ9Hflk5G5WIcjBi8atO925hC1p23QoD5lvq6w==
```

> Note that the message `SGVsbG8gV29ybGQ` is [Hello World](https://gchq.github.io/CyberChef/#recipe=To_Base64('A-Za-z0-9-_')&input=SGVsbG8gV29ybGQ) encoded in base64 URL safe.

### Comments
* ***There is no need to use a database. An in-memory dictionary to store the keys is enough.***
* ***You cannot use openssl CLI to perform the signature, you can use the native library that comes with the program language of choice, or you can use third party libraries.***
