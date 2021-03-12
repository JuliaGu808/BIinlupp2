CREATE TABLE DeviceTypes2(
	Id int not null identity(1,1) primary key,
	Type nvarchar(50) not null
)

CREATE TABLE GeoLocations2(
	 Id int not null identity(1,1) primary key,
	 Location nvarchar(50) not null,
	 Latitude float not null,
	 Longitude float not null
)

CREATE TABLE Roles2(
	 Id int not null identity(1,1) primary key,
	 RoleType nvarchar(50) not null
)
GO

CREATE TABLE Persons2(
	PersonId bigint not null identity(1,1) primary key,
	RoleId int not null references Roles2(Id),
	Name nvarchar(100) not null
)
GO

CREATE TABLE Students2(
	PersonId bigint not null primary key references Persons2(PersonId),
	School nvarchar(256) not null
)

CREATE TABLE Teachers2(
	PersonId bigint not null primary key references Persons2(PersonId),
	Office nvarchar(256) not null
)
GO

CREATE TABLE Devices2(
	Id char(17) not null primary key,
	DeviceTypeId int not null references DeviceTypes2(Id),
	GeoLocationId int not null references GeoLocations2(Id),
	PersonId bigint not null references Persons2(PersonId)
)
GO

CREATE TABLE DhtMeasurements2(
	Id bigint not null identity(1,1) primary key,
	DeviceId char(17) not null references Devices2(Id),
	MeasurementTime datetime not null,
	Temperature float not null,
	Humidity float not null
)

DROP TABLE DhtMeasurements2
DROP TABLE Devices2
DROP TABLE Teachers2
DROP TABLE Students2
DROP TABLE Persons2
DROP TABLE Roles2
DROP TABLE GeoLocations2
DROP TABLE DeviceTypes2
