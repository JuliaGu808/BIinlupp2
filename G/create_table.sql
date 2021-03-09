CREATE TABLE DeviceTypes(
	Id int not null identity(1,1) primary key,
	Type nvarchar(50) not null
)

CREATE TABLE GeoLocations(
	 Id int not null identity(1,1) primary key,
	 Latitude float not null,
	 Longitude float not null
)
GO

CREATE TABLE Devices(
	Id char(17) not null primary key,
	DeviceTypeId int not null references DeviceTypes(Id),
	GeoLocationId int not null references GeoLocations(Id)
)
GO

CREATE TABLE DhtMeasurements(
	Id bigint not null identity(1,1) primary key,
	DeviceId char(17) not null references Devices(Id),
	MeasurementTime datetime not null,
	Temperature float not null,
	Humidity float not null
)

DROP TABLE DhtMeasurements
DROP TABLE Devices
DROP TABLE GeoLocations
DROP TABLE DeviceTypes