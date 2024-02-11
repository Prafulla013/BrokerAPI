# Proshore-Assesment
# we have followed clean architecture on application.
#Here is my structure:
![image](https://github.com/Prafulla013/Proshore-Assesment/assets/42163808/235118c8-9f72-4db8-8705-644140b4ead2)


# Identity Framework for user management
![image](https://github.com/Prafulla013/Proshore-Assesment/assets/42163808/0835f42d-50ba-4b6a-ac05-a41c72a312fc)


#  integration testing
![image](https://github.com/Prafulla013/Proshore-Assesment/assets/42163808/8c7da204-7bcd-43ad-b53d-06c321d05a8d)

# Step to run application
# Make sure .net 7 sdk is installed on machine.
# Build Solution it will download all the dependency that needed for the application
# we have used Following package:
* EntityFrameworkCore 7.2
* MediatR 12.1
* SignalR 1.1 (For Notified if the associated role permission is updatd on application it will immediate response to UI through web socket)
* Azure.Storage.Blobs 12.16 (For broker image upload)
* EntityFrameworkCore.SqlServer
* xunit
* EntityFrameworkCore.InMemory( For test database)

# Change database connection string
# Then update migration it will up to date database 



