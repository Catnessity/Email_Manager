FROM httpd:2.4
WORKDIR "/usr/local/apache2/htdocs/"
COPY ./public-html/ .
#looks like the standard is giving access issues -> adjust chmod
RUN chmod -R 555 /usr/local/apache2/htdocs/
