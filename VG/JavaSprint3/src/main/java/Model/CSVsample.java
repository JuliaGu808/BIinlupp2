package Model;

import java.util.ArrayList;

public class CSVsample {
    public ArrayList<String> utc;
    public String latitude;
    public String longitude;
    public String location;
    public String deviceId;
    public CSVsample(ArrayList<String> utc, String latitude, String longitude, String location, String deviceId){
        this.utc=utc;
        this.latitude=latitude;
        this.longitude=longitude;
        this.location=location;
        this.deviceId=deviceId;
    }
}
