# Integration Test

The following document explains how to check the correct functionality of the Web API through the integration test.

> The source code for the integration test is in the `IntegrationTest` folder.

## How to run the integration test

The easiest way to test the web API is by running the `tideorg/backend-test` container. 

```bash
docker run --rm --net=host tideorg/backend-test
```
> `--net=host` is to allow the container to access the local network of the host where the Web API might be running

## How to change the URL

By default, the container assumes that the Web API is running on `http://localhost:8080`. If the API runs on a different path, the `URL` environment variable must be set. 

```bash
docker run --rm --net=host -e URL=https://safe.local tideorg/backend-test
```
> In the case that the API is running on `https://safe.local`