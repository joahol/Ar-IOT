//
//  SensorClient.swift
//  ARImageRecognition
//
//  Created by Joakim Holvik on 18/11/2018.
//  Copyright © 2018 Joakim Holvik. All rights reserved.
//

import Foundation

public class SensorClient{
    var sensorValue = 0;
    let serverIp="192.168.0.106:9091";
    let serverPort="9091";
    var serverData = "";
    
    
    public func Connect() {
     
        guard let url = URL(string: serverIp) else { return }
        URLSession.shared.dataTask(with: url){ (data, response, error) in
            if error != nil {
                
            }
            guard let serverdata = data else { return }
      
            self.serverData = serverdata.description
        
            }.resume()
        
    }
    
    public func retrieveSensor() ->String{
        return serverData
    }
    
   
}
