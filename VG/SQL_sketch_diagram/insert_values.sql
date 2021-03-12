DECLARE @roleType nvarchar(50)
DECLARE @roleId int
DECLARE @personId int
DECLARE @deviceTypeId int
DECLARE @geoLocationId int
DECLARE @name nvarchar(100)
DECLARE @school nvarchar(256)
DECLARE @office nvarchar(256)
DECLARE @deviceType nvarchar(50)
DECLARE @location nvarchar(50)
DECLARE @latitude float
DECLARE @longitude float
DECLARE @deviceId char(17)
DECLARE @measurementTime datetime
DECLARE @temperature decimal
DECLARE @humidity decimal

SET @roleType = 'student'
SET @name = 'Julia'
SET @school = 'Nackademin IoT 20'
SET @office = 'Stockholm'
SET @deviceType = 'dht'
SET @latitude = 59.2465954
SET @longitude = 18.0609175
SET @deviceId = 'A8:03:2A:EA:C9:84'
SET @location = 'Stockholm'
SET @measurementTime = GETDATE()
SET @temperature = 23.1
SET @humidity = 17

IF NOT EXISTS (SELECT Id FROM Roles2 WHERE RoleType = @roleType) 
	INSERT INTO Roles2 VALUES(@roleType) 
SELECT @roleId=Id FROM Roles2 WHERE RoleType=@roleType

IF NOT EXISTS (SELECT PersonId FROM Persons2 WHERE Name = @name) 
	INSERT INTO Persons2 VALUES(@roleId, @name) 
SELECT @personId=PersonId FROM Persons2 WHERE Name = @name

IF @roleType='student'
	IF NOT EXISTS (SELECT PersonId FROM Students2 WHERE PersonId = @personId) 
		INSERT INTO Students2 VALUES(@personId, @school) 
IF @roleType='teacher'
	IF NOT EXISTS (SELECT PersonId FROM Teachers2 WHERE PersonId = @personId) 
		INSERT INTO Teachers2 VALUES(@personId, @office) 

IF NOT EXISTS (SELECT Id FROM DeviceTypes2 WHERE Type = @deviceType) 
	INSERT INTO DeviceTypes2 VALUES(@deviceType) 
SELECT @deviceTypeId=Id FROM DeviceTypes2 WHERE Type = @deviceType

IF NOT EXISTS (SELECT Id FROM GeoLocations2 WHERE Latitude = @latitude AND Longitude = @longitude) 
	INSERT INTO GeoLocations2 VALUES(@location, @latitude, @longitude) 
SELECT @geoLocationId=Id FROM GeoLocations2 WHERE Latitude = @latitude AND Longitude = @longitude

IF NOT EXISTS (SELECT Id FROM Devices2 WHERE Id = @deviceId) 
	INSERT INTO Devices2 VALUES(@deviceId, @deviceTypeId, @geoLocationId, @personId) 

INSERT INTO DhtMeasurements2 VALUES (@deviceId, @measurementTime, @temperature, @humidity)


SELECT
	m.Id,
	m.MeasurementTime,
	m.Temperature,
	m.Humidity,
	d.Id as DeviceId,
	dt.Type as DeviceType,
	gl.Latitude,
	gl.Longitude,
	p.Name,
	st.School
FROM DhtMeasurements2 m
JOIN Devices2 d ON d.Id = m.DeviceId
JOIN DeviceTypes2 dt ON dt.Id = d.DeviceTypeId
JOIN GeoLocations2 gl ON gl.Id = d.GeoLocationId
JOIN Persons2 p ON p.PersonId = d.PersonId
JOIN Students2 st ON st.PersonId = p.PersonId
--WHERE m.DeviceId = 'A8:03:2A:EA:C9:84'

SELECT * FROM Roles2

