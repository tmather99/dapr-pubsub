TAG = v1
SRC = ../../DemoMicroservices

.PHONY: ingress

#
# cluster targets
#
cluster:
	kind create cluster --name cluster --config=kind/cluster.yml

rm-cluster:
	kind delete cluster --name cluster

#
# init targets
#
init: \
	helm-repos \
	nginx-kind \
	metrics-server

helm-install: \
	dapr-install \
	dapr-dashboard-install \
	rabbitmq

helm-uninstall: \
	dapr-uninstall \
	dapr-dashboard-uninstall \
	rm-rabbitmq

helm-repos:
	helm repo add bitnami https://charts.bitnami.com/bitnami
	helm repo add jenkins https://charts.jenkins.io
	helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
	helm repo add grafana https://grafana.github.io/helm-charts
	helm repo add metrics-server https://kubernetes-sigs.github.io/metrics-server/
	helm repo add dapr https://dapr.github.io/helm-charts/
	helm repo add jetstack https://charts.jetstack.io
	helm repo add open-telemetry https://open-telemetry.github.io/opentelemetry-helm-charts
	helm repo add ckotzbauer https://ckotzbauer.github.io/helm-charts
	helm repo update

up: helm-install \
	apps \
	dapr-configs \
	seq \
	secrets \
	ingress

down: rm-apps \
	  rm-dapr-configs \
	  rm-seq \
	  rm-secrets \
	  rm-ingress \
	  helm-uninstall \
	  clean	

#
# secrets targets
#
secrets:
	kubectl apply -f ingress/assistdevops-tls.yml

# kubectl delete secret dapr-assistdevops-tls -n dapr-system
rm-secrets: \
	-kubectl delete -f ingress/assistdevops-tls.yml

#
# ingress targets
#

# https://raw.githubusercontent.com/kubernetes/ingress-nginx/main/deploy/static/provider/kind/deploy.yaml
nginx-kind:
	kubectl apply -f ingress/nginx-controller-kind-v1.8.1.yml

rm-nginx-kind:
	kubectl delete -f ingress/nginx-controller-kind-v1.8.1.yml
	
apps-ingress:
	kubectl apply -f ingress/apps.yml

rm-apps-ingress:
	kubectl delete -f ingress/apps.yml

rabbitmq-ingress:
	kubectl apply -f ingress/rabbitmq.yml

rm-rabbitmq-ingress:
	-kubectl delete -f ingress/rabbitmq.yml

utils-ingress:
	kubectl apply -f ingress/utils.yml

rm-utils-ingress:
	-kubectl delete -f ingress/utils.yml

dapr-dashboard-ingress:
	kubectl apply -f ingress/dapr-dashboard.yml

rm-dapr-dashboard-ingress:
	-kubectl delete -f ingress/dapr-dashboard.yml

ingress: \
	dapr-dashboard-ingress \
	rabbitmq-ingress \
	utils-ingress \
	apps-ingress

rm-ingress: \
	rm-dapr-dashboard-ingress \
	rm-rabbitmq-ingress \
	rm-utils-ingress \
	rm-apps-ingress

seq:
	kubectl apply -f utils/seq.yml

rm-seq:
	kubectl delete -f utils/seq.yml

rabbitmq:
	helm upgrade --install rabbitmq bitnami/rabbitmq --create-namespace \
	--set auth.username=user,auth.password=bitnami,auth.erlangCookie=secretcookie \
	--set replicaCount=1,metrics.enabled=true --wait
	
rm-rabbitmq:
	helm delete rabbitmq

#
# darp targets
#
dapr-install:	
	helm upgrade --install dapr dapr/dapr --version=1.11.2 -n dapr-system --create-namespace --wait

dapr-uninstall:
	helm uninstall dapr -n dapr-system

dapr-dashboard-install:
	helm upgrade --install dapr-dashboard dapr/dapr-dashboard -n dapr-system --wait

dapr-dashboard-uninstall:
	helm uninstall dapr-dashboard -n dapr-system

dapr-pubsub-rabbitmq:
	kubectl apply -f dapr/pubsub-rabbitmq.yml 

rm-dapr-pubsub-rabbitmq:
	kubectl delete -f dapr/pubsub-rabbitmq.yml 

dapr-configs: \
    dapr-pubsub-rabbitmq

rm-dapr-configs: \
    rm-dapr-pubsub-rabbitmq

#
# apps targets
#
consumer:
	helm install consumer apps/consumer

rm-consumer:
	helm delete consumer  

producer:
	helm install producer apps/producer 

rm-producer:
	helm delete producer 

apps: \
	consumer \
	producer

rm-apps: \
	rm-consumer \
	rm-producer

metrics-server:
	helm upgrade --install --set 'args={--kubelet-insecure-tls}' \
	             -n kube-system metrics metrics-server/metrics-server

rm-metrics-server:
	helm delete metrics -n kube-system 
