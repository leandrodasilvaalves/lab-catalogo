#!/bin/bash

# criar deployment rabbitmq
helm install rabbitmq  bitnami/rabbitmq

# excluir deployment rabbitmq
# helm uninstall rabbitmq