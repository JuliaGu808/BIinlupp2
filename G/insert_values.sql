DECLARE @deviceId char(17)
DECLARE @measurementTime datetime
DECLARE @temperature decimal
DECLARE @humidity decimal

SET @deviceId = 'A8:03:2A:EA:C9:84'
SET @measurementTime = GETDATE()
SET @temperature = 23.1
SET @humidity = 17

INSERT INTO DhtMeasurements VALUES (@deviceId, @measurementTime, @temperature, @humidity)
SELECT * FROM DhtMeasurements



IF NOT EXISTS (SELECT Id FROM DeviceTypes WHERE Type = @deviceType) INSERT INTO DeviceTypes OUTPUT inserted.id VALUES(@deviceType) ELSE SELECT * FROM DeviceTypes
IF NOT EXISTS (SELECT Id FROM GeoLocations WHERE Latitude = @latitude AND Longitude = @longitude) INSERT INTO GeoLocations OUTPUT inserted.Id VALUES(@latitude, @longitude) ELSE SELECT * FROM GeoLocations WHERE Latitude = @latitude AND Longitude = @longitude
IF NOT EXISTS (SELECT Id FROM Devices WHERE Id = @deviceId) INSERT INTO Devices OUTPUT inserted.Id VALUES(@deviceId, @deviceTypeId, @geoLocationId) ELSE SELECT * FROM Devices WHERE Id = @deviceId
INSERT INTO DhtMeasurements VALUES (@deviceId, @measurementTime, @temperature, @humidity)

SELECT
	m.Id,
	m.MeasurementTime,
	m.Temperature,
	m.Humidity,
	d.Id as DeviceId,
	dt.Type as DeviceType,
	gl.Latitude,
	gl.Longitude
FROM DhtMeasurements m
JOIN Devices d ON d.Id = m.DeviceId
JOIN DeviceTypes dt ON dt.Id = d.DeviceTypeId
JOIN GeoLocations gl ON gl.Id = d.GeoLocationId
--WHERE m.DeviceId = 'A8:03:2A:EA:C9:84'