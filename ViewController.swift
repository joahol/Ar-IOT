//
//  ViewController.swift
//  ARImageRecognition
//
//  Created by Joakim Holvik on 24.10.2018.
//  Copyright © 2018 Joakim Holvik. All rights reserved.
//

import UIKit
import SceneKit
import ARKit

class ViewController: UIViewController, ARSCNViewDelegate {

    @IBOutlet var sceneView: ARSCNView!
    @IBOutlet weak var label: UILabel!
    let ipString = "192.168.0.106:9091/"
    let scaleFact = 0.15
    let verbose = true
    let debug = true
/*
    lazy var r2d2Node: SCNNode = {
        guard let scene = SCNScene(named: "art.scnassets/r2d2.scn"),
            let node = scene.rootNode.childNode(withName: "r2d2", recursively: false) else {
                print("something happend in loading r2d2")
                return SCNNode() }
        node.scale = SCNVector3(scaleFact, scaleFact, scaleFact)
        node.eulerAngles.x = -.pi / 2
        return node
    }()
  */
    func configurate(){
        var message = ""
        sceneView.autoenablesDefaultLighting = true
        sceneView.automaticallyUpdatesLighting = true
        guard let refImages = ARReferenceImage.referenceImages(inGroupNamed: "AR Resources", bundle:nil) else {
            message = "could not load reference images"
            return }
         let arConfig = ARWorldTrackingConfiguration()
       arConfig.detectionImages = refImages
    
        sceneView.delegate = self
        let arOptions: ARSession.RunOptions = [.resetTracking, .removeExistingAnchors]
        sceneView.session.run(arConfig, options: arOptions)
            print(message)
      
      }
    
    override func viewDidLoad() {
        super.viewDidLoad()
      configurate()
       
        //let scene = SCNScene(named: "art.scnassets/r2d2.scn")!
     label.text = "Scanning for recognizable pictures"
       // sceneView.scene = scene
    }
    
    override func viewWillAppear(_ animated: Bool) {
        super.viewWillAppear(animated)
        
       //let configuration = ARWorldTrackingConfiguration()
        //sceneView.session.run(configuration)
    }
    
    override func viewWillDisappear(_ animated: Bool) {
        super.viewWillDisappear(animated)
        sceneView.session.pause()
    }

  
    func session(_ session: ARSession, didFailWithError error: Error) {
   
    }
    
    func sessionWasInterrupted(_ session: ARSession) {
    
        
    }
    
    func sessionInterruptionEnded(_ session: ARSession) {
     
    }


func renderer(_ renderer: SCNSceneRenderer, didAdd node: SCNNode, for anchor: ARAnchor){
    var message = ""
    DispatchQueue.main.async {
        guard let imgAnchor = anchor as? ARImageAnchor,
             let imgName = imgAnchor.referenceImage.name else {
                message = "could not get referenceimage to anchor"
                return }
        message += "image " + imgName
        let plane = SCNPlane(width: imgAnchor.referenceImage.physicalSize.width, height: imgAnchor.referenceImage.physicalSize.height)
       let topNode = SCNNode(geometry: plane)
       let parameter="/?1=1"
        if let url = URL(string: "http://192.168.0.106:9091"+parameter) {
            if let data = try? Data(contentsOf: url) {
                message = String(data: data, encoding: .utf8) ?? "!"
              
                
                print(message)
             self.addR2D2(x: topNode.position.x ,y: topNode.position.y,z: topNode.position.z)
                self.addLabel(x: topNode.position.x ,y: topNode.position.y,z: topNode.position.z-0.2, lblText: message)
                
            } else {
                print("Why did i not get any response from server?")
            }
        }
            topNode.opacity = 0.1
        topNode.position.z = -1
        

        node.addChildNode(topNode)
    }
    
    }
  
    func addR2D2(x: Float = 0, y: Float = 0, z: Float = 0){
        guard let r2s = SCNScene(named: "art.scnassets/r2d2.dae") else { return }
        let r2Node = SCNNode()
        for chn in r2s.rootNode.childNodes{
            r2Node.addChildNode(chn)
        }
        r2Node.scale = SCNVector3(0.01,0.01,0.01)
        r2Node.position = SCNVector3(x, y, -0.5);
     
    
        sceneView.scene.rootNode.addChildNode(r2Node)
        
    }
 
    func addLabel(x: Float = 0, y: Float = 0, z: Float = 0 , lblText: String = "?" ){
        let labelNode = SCNText()
        labelNode.string = lblText
        
        labelNode.font = UIFont.systemFont(ofSize: 1)
        labelNode.isWrapped = true
        labelNode.containerFrame = CGRect(x: 1, y: 0, width: 16, height: 10)
        labelNode.alignmentMode = CATextLayerAlignmentMode.center.rawValue
        labelNode.materials.first?.diffuse.contents = UIColor.black.cgColor
        var tNode = SCNNode(geometry: labelNode)
        tNode.position = SCNVector3(x,y,z)
        tNode.scale = SCNVector3(0.1,0.1,0.1)
        sceneView.scene.rootNode.addChildNode(tNode)
    }
}

