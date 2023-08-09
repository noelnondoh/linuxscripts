// Step 1: Install Java
sudo apt update
sudo apt install openjdk-11-jdk -y
// sudo apt install default-jdk
// java -version

// Step 2: Add Jenkins Repository
curl -fsSL https://pkg.jenkins.io/debian-stable/jenkins.io.key | sudo tee /usr/share/keyrings/jenkins-keyring.asc > /dev/null
echo deb [signed-by=/usr/share/keyrings/jenkins-keyring.asc] https://pkg.jenkins.io/debian-stable binary/ | sudo tee /etc/apt/sources.list.d/jenkins.list > /dev/null
/
/ Step 3: Install Jenkins
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
sudo apt-get install openssh-server
sudo systemctl status ssh

// Step 10: Installing dotnet
wget https://dot.net/v1/dotnet-install.sh -O dotnet-install.sh
sudo chmod +x ./dotnet-install.sh

./dotnet-install.sh --version latest

./dotnet-install.sh --version latest --runtime aspnetcore

// Vous pouvez installer une version majeure spécifique avec le paramètre --channel pour indiquer la version spécifique. La commande suivante installe le SDK .NET 7.0.
// Bash
./dotnet-install.sh --channel 7.0