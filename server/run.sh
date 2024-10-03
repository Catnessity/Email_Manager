
sudo docker stop c-mail
sudo docker build -t page-server .
sudo docker run -dit --rm --name c-mail -p 8080:80 page-server 