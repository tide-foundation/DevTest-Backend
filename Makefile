.PHONY: run-docker docker push

run-docker:
	docker run --rm --net=host tideorg/backend-test

docker:
	@docker build -f IntegrationTest/Dockerfile -t tideorg/backend-test:latest . > /dev/null
	@[ -n "$$tag" ] && docker tag tideorg/backend-test "tideorg/backend-test:$$tag" || \
	echo "Epecify the 'tag' to assign it to the container"

push:
	id=$$(docker image ls | grep tideorg/backend-test | head -n 1 | awk '{ printf $$3 }'); \
	tag=$$(docker image ls | grep "$$id" | grep -v 'latest' | head -n 1 | awk '{ printf $$2 }'); \
	docker push "tideorg/backend-test:latest"; \
	if [ -n "$$tag" ]; then docker push "tideorg/backend-test:$$tag"; fi
