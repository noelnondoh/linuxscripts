// Step 1: Install Java
lsb_release -a // Check Ubuntu version
sudo apt update
sudo apt install openjdk-11-jdk -y
// sudo apt install default-jdk
// java -version

// Step 2: Add Jenkins Repository
curl -fsSL https://pkg.jenkins.io/debian-stable/jenkins.io.key | sudo tee /usr/share/keyrings/jenkins-keyring.asc > /dev/null
echo deb [signed-by=/usr/share/keyrings/jenkins-keyring.asc] https://pkg.jenkins.io/debian-stable binary/ | sudo tee /etc/apt/sources.list.d/jenkins.list > /dev/null

// Step 3: Install Jenkins
sudo apt update
sudo apt install jenkins -y
// To check status, type:
// sudo systemctl status jenkins
// To start the web server when it is stopped, type:
// sudo systemctl enable --now jenkins

// sudo ufw status
sudo ufw enable
sudo ufw allow 8080
sudo cat /var/lib/jenkins/secrets/initialAdminPassword

// Step 4: Install Maven
sudo apt install maven
// mvn -version 

// Step 5: Installing Apache
sudo apt install apache2
// sudo ufw app list
sudo ufw allow 'Apache'

// hostname -I

// To check status, type:
// sudo systemctl status apache2
// To stop your web server, type:
// sudo systemctl stop apache2
// To start the web server when it is stopped, type:
sudo systemctl start apache2
// To stop and then start the service again, type:
sudo systemctl restart apache2
// If you are simply making configuration changes, Apache can often reload without dropping connections. To do this, use this command:
sudo systemctl reload apache2
// By default, Apache is configured to start automatically when the server boots. If this is not what you want, disable this behavior by typing:
sudo systemctl disable apache2
// To re-enable the service to start up at boot, type:
sudo systemctl enable apache2

// Step 6: Installing Docker
sudo apt update
sudo apt install apt-transport-https ca-certificates curl software-properties-common
curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
sudo apt update

// Make sure you are about to install from the Docker repo instead of the default Ubuntu repo:
apt-cache policy docker-ce
// Notice that docker-ce is not installed, but the candidate for installation is from the Docker repository for Ubuntu 22.04 (jammy).
// Finally, install Docker:
sudo apt install docker-ce
// Docker should now be installed, the daemon started, and the process enabled to start on boot. Check that it’s running:
sudo systemctl status docker

// Executing the Docker Command Without Sudo (Optional)
sudo usermod -aG docker ${USER}
su - ${USER}
    groups

// If you need to add a user to the docker group that you’re not logged in as, declare that username explicitly using:
sudo usermod -aG docker username

Step 7: Installing Triviy
sudo apt-get install wget apt-transport-https gnupg lsb-release
wget -qO - https://aquasecurity.github.io/trivy-repo/deb/public.key | sudo apt-key add -
echo deb https://aquasecurity.github.io/trivy-repo/deb $(lsb_release -sc) main | sudo tee -a /etc/apt/sources.list.d/trivy.list
sudo apt-get update
sudo apt-get install trivy

// Step 8: Installing SonarQube
wget -q https://www.postgresql.org/media/keys/ACCC4CF8.asc -O - | sudo apt-key add -
sudo sh -c 'echo "deb http://apt.postgresql.org/pub/repos/apt/ `lsb_release -cs`-pgdg main" >> /etc/apt/sources.list.d/pgdg.list'
sudo apt update
add postgresql repository
sudo apt install postgresql-13
install postgresql
sudo systemctl is-enabled postgresql
sudo systemctl status postgresql

check postgresql

// With the PostgreSQL installed on your server, you are ready to set up a new database and user for the SonarQube via the PostgreSQL shell.
sudo -u postgres psql

// Now, run the following PostgreSQL queries to create a new database and user for SnonarQube. In this example, you will create the PostgreSQL database and user 'sonarqube'. And be sure to change the password with a strong password.

CREATE USER sonarqube WITH PASSWORD 'Password';
CREATE DATABASE sonarqube OWNER sonarqube;
GRANT ALL PRIVILEGES ON DATABASE sonarqube TO sonarqube;

create database

// Next, run the following queries to check the list of databases and users on the PostgreSQL server.
// \l
// \du

// Lastly, log out from PostgreSQL using the query below.
// \q

sudo useradd -b /opt/sonarqube -s /bin/bash sonarqube

// Next, open the file /etc/sysctl.conf using nano editor.
sudo nano /etc/sysctl.conf

// Add the following configuration to the bottom of the line. The SonarQube required the kernel parameter vm.max_map_count to be greater than '524288' and the fx.file-max to be greater than '131072'.
// vm.max_map_count=524288
// fs.file-max=131072

sudo sysctl --system
ulimit -n 131072
ulimit -u 8192
sudo nano /etc/security/limits.d/99-sonarqube.conf
// Add the following configuration to the file.
sonarqube   -   nofile   131072
sonarqube   -   nproc    8192

sudo apt install unzip software-properties-common wget
wget https://binaries.sonarsource.com/Distribution/sonarqube/sonarqube-9.6.1.59531.zip
unzip sonarqube-9.6.1.59531.zip
mv sonarqube-9.6.1.59531 /opt/sonarqube
sudo chown -R sonarqube:sonarqube /opt/sonarqube

sudo nano /opt/sonarqube/conf/sonar.properties

// sonar.jdbc.username=sonarqube
// sonar.jdbc.password=Password

// sonar.jdbc.url=jdbc:postgresql://localhost:5432/sonarqube

// Uncomment the following configuration to set up the max heap memory size for the elasticsearch process. In his example, the max heap will be 512 MB.

sonar.search.javaOpts=-Xmx512m -Xms512m -XX:MaxDirectMemorySize=256m -XX:+HeapDumpOnOutOfMemoryError

// sonar.web.host=127.0.0.1
// sonar.web.port=9000
// sonar.web.javaAdditionalOpts=-server

// sonar.log.level=INFO
// sonar.path.logs=logs

sudo nano /etc/systemd/system/sonarqube.service

// Add the following configuration to the file.

// [Unit]
// Description=SonarQube service
// After=syslog.target network.target

// [Service]
// Type=forking
// ExecStart=/opt/sonarqube/bin/linux-x86-64/sonar.sh start
// ExecStop=/opt/sonarqube/bin/linux-x86-64/sonar.sh stop
// User=sonarqube
// Group=sonarqube
// Restart=always
// LimitNOFILE=65536
// LimitNPROC=4096

// [Install]
// WantedBy=multi-user.target

sudo systemctl daemon-reload

// After that, start and enable the 'sonarqube.service' via the systemctl command below.
sudo systemctl start sonarqube.service
sudo systemctl enable sonarqube.service


// Lastly, verify the 'sonarqube.service' status using the following command and make sure its status is running.
sudo systemctl status sonarqube.service

// Step 9: Installing OpenSSH
sudo apt update
sudo apt upgrade
sudo apt-get install openssh-server openssh-client
sudo systemctl status ssh
sudo ufw allow ssh

// Step 10: Installing dotnet
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
sudo chmod +x ./dotnet-install.sh
./dotnet-install.sh --version latest
// or
./dotnet-install.sh --version latest --runtime aspnetcore

// Vous pouvez installer une version majeure spécifique avec le paramètre --channel pour indiquer la version spécifique. La commande suivante installe le SDK .NET 7.0.
./dotnet-install.sh --channel 7.0

// Step 11: Installing Terraform
sudo apt-get update && sudo apt-get install -y gnupg software-properties-common
wget -O- https://apt.releases.hashicorp.com/gpg | \
gpg --dearmor | \
sudo tee /usr/share/keyrings/hashicorp-archive-keyring.gpg

gpg --no-default-keyring \
--keyring /usr/share/keyrings/hashicorp-archive-keyring.gpg \
--fingerprint

echo "deb [signed-by=/usr/share/keyrings/hashicorp-archive-keyring.gpg] \
https://apt.releases.hashicorp.com $(lsb_release -cs) main" | \
sudo tee /etc/apt/sources.list.d/hashicorp.list
sudo apt update
sudo apt-get install terraform
// terraform -help

// Step 12: Installing Ansible and Boto
sudo apt-add-repository ppa:ansible/ansible
sudo apt update
sudo apt install ansible

// sudo apt install software-properties-common--
// sudo add-apt-repository --yes --update ppa:ansible/ansible--
// sudo apt install ansible -y
// sudo apt install python3-boto3
// sudo apt install python3-botocore

// Step 13: Free space
sudo apt-get autoremove
sudo apt-get autoclean
sudo apt-get clean
sudo journalctl --vacuum-time=3d
// du -sh ~/.cache/thumbnails
rm -rf ~/.cache/thumbnails/*

// Step 14: Installing MicroK8s
// Get set up for snaps
sudo apt update
sudo apt install snapd
sudo snap install microk8s --classic

// microk8s status --wait-ready
// Turn on the services you want
microk8s enable dashboard dns registry istio

// Try microk8s enable --help for a list of available services and optional features. microk8s disable <name> turns off a service.
// Start using Kubernetes
microk8s kubectl get all --all-namespaces

// If you mainly use MicroK8s you can make our kubectl the default one on your command-line with alias mkctl="microk8s kubectl". Since it is a standard upstream kubectl, you can also drive other Kubernetes clusters with it by pointing to the respective kubeconfig file via the --kubeconfig argument.
// Access the Kubernetes dashboard
microk8s dashboard-proxy

// Start and stop Kubernetes to save battery
microk8s start 
microk8s stop

// Step 15: Static IP address assignment
// To configure your system to use static address assignment, create a netplan configuration in the file /etc/netplan/99_config.yaml.
// network:
//   version: 2
//   renderer: networkd
//   ethernets:
//     eth0:
//       addresses:
//         - 10.10.10.2/24
//       routes:
//         - to: default
//           via: 10.10.10.1
//       nameservers:
//           search: [mydomain, otherdomain]
//           addresses: [10.10.10.1, 1.1.1.1]

// The configuration can then be applied using the netplan command.
sudo netplan apply


// Step 16: Install Kubernetes Cluster on Ubuntu 20.04
// Lab setup:
    //     Machine 1 – K8s-master – 192.168.1.40
    //     Machine 2 – K8s-node-0 – 192.168.1.41
    //     Machine 3 – K8s-node-1 – 192.168.1.42

sudo hostnamectl set-hostname "k8s-master"     // Run this command on master node
sudo hostnamectl set-hostname "k8s-node-0"     // Run this command on node-0
sudo hostnamectl set-hostname "k8s-node-1"     // Run this command on node-1

// Add the following entries in sudo nano /etc/hosts files on each node,
    // 192.168.1.40    k8s-master
    // 192.168.1.41    k8s-node-0
    // 192.168.1.42    k8s-node-1

// Disable swap and add following kernel module on all the nodes ( master + worker nodes).
// Disable swap, edit /etc/fstab file and comment out the line which includes entry either swap partition or swap file.
sudo vi /etc/fstab

sudo swapoff -a

sudo tee /etc/modules-load.d/containerd.conf <<EOF
overlay
br_netfilter
EOF

sudo modprobe overlay
sudo modprobe br_netfilter

sudo tee /etc/sysctl.d/kubernetes.conf<<EOF
net.bridge.bridge-nf-call-ip6tables = 1
net.bridge.bridge-nf-call-iptables = 1
net.ipv4.ip_forward = 1
EOF

sudo sysctl --system

// Install Containerd Runtime on All Nodes
sudo apt install -y curl gnupg2 software-properties-common apt-transport-https ca-certificates

sudo curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmour -o /etc/apt/trusted.gpg.d/docker.gpg
sudo add-apt-repository "deb [arch=amd64] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable"

sudo apt update
sudo apt install -y containerd.io

// Configure the contianerd using follwing command,
containerd config default | sudo tee /etc/containerd/config.toml >/dev/null 2>&1
sudo sed -i 's/SystemdCgroup \= false/SystemdCgroup \= true/g' /etc/containerd/config.toml
sudo systemctl restart containerd && sudo systemctl enable containerd
sudo systemctl status containerd

// Install Kubectl, kubelet and kubeadm on all nodes
sudo apt install -y apt-transport-https curl 
curl -s https://packages.cloud.google.com/apt/doc/apt-key.gpg | sudo gpg --dearmour -o /etc/apt/trusted.gpg.d/kubernetes-xenial.gpg
sudo apt-add-repository "deb http://apt.kubernetes.io/ kubernetes-xenial main"
sudo apt update $ sudo apt install -y kubelet kubeadm kubectl


// Initialize Kubernetes Cluster using kubeadm
// Login to your master node (k8s-master) and run below ‘kubeadm init‘ command to initialize Kubernetes cluster,
sudo kubeadm init --control-plane-endpoint=k8s-master

// Once the cluster is initialized successfully, we will get the following output
// Kubeadm-Init-Ubuntu-20-04

mkdir -p $HOME/.kube
sudo cp -i /etc/kubernetes/admin.conf $HOME/.kube/config
sudo chown $(id -u):$(id -g) $HOME/.kube/config

// Add Worker Nodes to Cluster
// If you want to add worker nodes to your Kubernetes cluster, log in to each worker node and run the ‘kubeadm join’ command you saved from Step 5.
// Copy “kubeadm join” command and paste it on both nodes (worker nodes).
sudo kubeadm join k8s-master:6443 --token kgiecl.fs4nuhs9nx3g523f --discovery-token-ca-cert-hash sha256:2550ea78e1a6a8b9f93c1e0eaeaa47407c67f9c9a6e4a4865c8c4bc982699ce7

// Join-Worker-Nodes-K8s-Cluster-Ubuntu
// Now, verify the nodes status from the master node,  run “kubectl get nodes” c
kubectl get nodes

// Deploy Calico Pod Network Add-on
// From the master node, run the following command to install Calico pod network add-on,
kubectl apply -f https://raw.githubusercontent.com/projectcalico/calico/v3.26.1/manifests/calico.yaml
Install-Calico-k8s-Ubuntu

kubectl get nodes

// To enable bash completion feature on your master node, execute the followings

echo 'source <(kubectl completion bash)' >>~/.bashrc
source .bashrc

// Test and Verify Kubernetes Cluster
kubectl create deployment nginx-web --image=nginx
kubectl get deployments.apps
kubectl get  pods

// Let’s scale up the deployment, set replicas as 4. Run the following command,
kubectl scale --replicas=4 deployment nginx-web
kubectl get deployments.apps nginx-web
kubectl describe deployments.apps nginx-web
kubectl run http-web --image=httpd --port=80


// Create a service using beneath command and expose above created pod on port 80,
kubectl expose pod http-web --name=http-service --port=80 --type=NodePort
kubectl get service http-service

// Download Metrics Server Manifest
curl -LO https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/components.yaml

// Download-Metrics-Server-Yaml-file-Curl-Command
curl https://github.com/kubernetes-sigs/metrics-server/releases/latest/download/high-availability-1.21+.yaml

// Modify Metrics Server Yaml File
vi components.yaml

// Find the args section under the container section, add the following line:
- --kubelet-insecure-tls

// Under the spec section, add following parameter,
hostNetwork: true

// Deploy Metrics Server
kubectl apply -f components.yaml

// Verify Metrics Server Deployment
kubectl get pods -n kube-system

// Test Metrics Server Installation
kubectl top nodes

// To view pods resource utilization of your current namespace or specific namespace, run
kubectl top pod

kubectl top pod -n kube-system

// Install Secret for DockerHub and PostgreSQL
kubectl create secret docker-registry regcred --docker-server=https://index.docker.io/v1/ --docker-username=vsnondoh --docker-password=Omerta@1988 --docker-email=noelnondoh@gmail.com
kubectl create secret generic pgpassword --from-literal PGPASSWORD=Omerta@1988

// Verify the secrets
kubectl get secret regcred --output=yaml
kubectl get secret

// Install NGINX Ingress Controller
kubectl apply -f https://raw.githubusercontent.com/kubernetes/ingress-nginx/controller-v1.8.1/deploy/static/provider/cloud/deploy.yaml

// Copy Deployments Files using SSH
scp -r \Users\root\source\repos\Mini_CRUD_App\kubernetes\ noel@192.168.1.20:/home/noel/k8s

