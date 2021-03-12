package com.example.demo;

import Model.CSVsample;
import Model.Sample;
import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.web.client.RestTemplate;

import java.io.*;
import java.lang.reflect.Array;
import java.nio.charset.StandardCharsets;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Optional;
import java.util.Set;

@SpringBootApplication
public class WheatherApplication {

    public static void main(String[] args) {

        SpringApplication.run(WheatherApplication.class, args);
        System.out.println("Hello World !");
        WheatherApplication app = new WheatherApplication();
        RestTemplate restTemplate = new RestTemplate();
        String url = "https://juliafunctiondemo2.azurewebsites.net/api/GetAllFromCosmos?";
                //"http://localhost:7071/api/GetAllFromCosmos";
                //= "https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/10/lat/59/data.json";

        String res = restTemplate.getForObject(url, String.class);
        res = "{begin:"+res+"}";
        ArrayList<CSVsample> csvsamples = app.converJsonToList(res);
        ArrayList<String> loc_temp = new ArrayList<>();
        csvsamples.forEach(each->{
            String lat = each.latitude;
            String longi = each.longitude;
            String temp_url = "https://opendata-download-metfcst.smhi.se/api/category/pmp3g/version/2/geotype/point/lon/"
                    +longi+"/lat/"+lat+"/data.json";
            String dht = restTemplate.getForObject(temp_url, String.class);
            String locTemp = each.location+","+app.showResult(dht);
            loc_temp.add(locTemp);
            //System.out.println(app.showResult(dht));
        });
        //loc_temp.forEach(el->System.out.println(el));
        app.SaveToCSV(loc_temp);

    }

    public ArrayList<CSVsample> converJsonToList (String res) throws JSONException {
        JSONObject obj = new JSONObject(res);
        JSONArray jArray = obj.getJSONArray("begin");
        ArrayList<String> listdata = new ArrayList<String>();
        if (jArray != null) {
            for (int i=0;i<jArray.length();i++){
                listdata.add(jArray.get(i).toString());
            }
        }
        //{"utc":"2021-03-12T06:24:52Z","latitude":"59.3293","location":"Stockholm","deviceId":"A8:03:2A:EA:C9:84","longitude":"18.0686"}
        ArrayList<Sample> samples = new ArrayList<>();
        ArrayList<CSVsample> csvsamples = new ArrayList<>();
        Set locations = new HashSet();
        listdata.stream().forEach(each->{
            String[] lists = each.split(",");
            String[] firsts = lists[0].split("\"");
            String utc = firsts[3];
            String[] seconds = lists[1].split("\"");
            String latitude = seconds[3];
            String[] thirds = lists[2].split("\"");
            String location = thirds[3];
            locations.add(location);
            String[] fourth = lists[3].split("\"");
            String deviceId = fourth[3];
            String[] fivth = lists[4].split("\"");
            String longitude = fivth[3];
            Sample sample = new Sample(utc, latitude, longitude, location, deviceId);
            samples.add(sample);
        });
        ArrayList<String> listOfLocs = new ArrayList<>(locations);

        listOfLocs.stream().forEach(each-> {
            ArrayList<String> utcs = new ArrayList<>();
            samples.stream().filter(el->el.location.equals(each)).forEach(sample->{
                utcs.add(sample.utc);

            });
            Sample sam = samples.stream().filter(el->el.location.equals(each)).findFirst().get();

            CSVsample csv = new CSVsample(utcs, sam.latitude, sam.longitude, sam.location, sam.deviceId);
            csvsamples.add(csv);
        });

        //csvsamples.forEach(each->System.out.println(each.utc.size()));
        return csvsamples;
    }


    public String showResult(String res) throws JSONException {
        String result = "t";
        JSONObject obj = new JSONObject(res);
        JSONArray oneHourLater = obj.getJSONArray("timeSeries");
        JSONArray parameters = oneHourLater.getJSONObject(0).getJSONArray("parameters");
        for(int i=0; i<parameters.length(); i++){
            String getAllNames = parameters.getJSONObject(i).getString("name");
            if(getAllNames.equals("t")){
                JSONArray temperature = parameters.getJSONObject(i).getJSONArray("values");
                result+=temperature.get(0).toString();
                //System.out.print(temperature.get(0) + "C. Weather will be ");
            }
        }
        return result;
    }

    public void SaveToCSV(ArrayList<String> text){
        String csvFilePath = "dht.csv";

        //try (PrintWriter writer = new PrintWriter(new File("test.csv"))) {
        try (FileOutputStream file = new FileOutputStream(csvFilePath);
             OutputStreamWriter fileWriter = new OutputStreamWriter(file, StandardCharsets.UTF_8);) {

            StringBuilder sb = new StringBuilder();
            sb.append("Location");
            sb.append(',');
            sb.append("Temperature");
            sb.append('\n');
            text.forEach(each->{
                sb.append(each);
                sb.append('\n');

                // writer.write(sb.toString());
            });
            try {

                //fileWriter.write("\u0627\u0644\u0627\u0633\u0645\u0020\u060c\u0020\u0627\u0644\u0633\u0646\u0020\u060c\u0020\u0627\u0644\u0639\u0646\u0648\u0627\u0646");
                fileWriter.write(sb.toString());
            } catch (IOException e) {
                e.printStackTrace();
            }
            System.out.println("done!");

        } catch (FileNotFoundException e) {
            System.out.println(e.getMessage());
        } catch (IOException e) {
            e.printStackTrace();
        }
    }

}
