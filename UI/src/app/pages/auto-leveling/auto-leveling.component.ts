import { Component, OnInit } from "@angular/core";
import * as THREE from 'three';

@Component({
    selector: 'app-auto-leveling',
    templateUrl: './auto-leveling.component.html',
    styleUrls: ['./auto-leveling.component.scss']
})
export class AutoLevelingComponent implements OnInit
{
    grid: Array<gridPoint> = [];
    // scene: any;
    // camera: any;
    // renderer: any;
    // cube: any;
    // geometry: any;
    // material: any;
    cameraControl: CameraControl | undefined;

    ngOnInit(): void {
        var scene = new THREE.Scene();
        var camera = new THREE.PerspectiveCamera( 75, window.innerWidth/window.innerHeight, 0.1, 1000 );
        //camera.position.set(0,2.5,2.5);
        //camera.lookAt(new THREE.Vector3(0,0,0));

        var renderer = new THREE.WebGLRenderer();
        renderer.setClearColor( 0xffffff, 0);
        renderer.setSize( window.innerWidth - 25, window.innerHeight - 100 );
        document.body.appendChild( renderer.domElement );

        var cicrleGeometry = new THREE.SphereGeometry(0.1,32,24);
        var material = new THREE.MeshBasicMaterial( { color: 0x000000 } );
        for(var x = 0; x < 101; x++)
        {
            for(var y = 0; y < 101; y++)
            {
                var obj = { X: x, Y:y, Z:0, Obj: new THREE.Mesh(cicrleGeometry, material) };
                obj.Obj.position.set(x,y,0);
                this.grid.push({ X: x, Y:y, Z:0, Obj: obj });
                scene.add( obj.Obj );
            }
        }

        camera.position.z = 50;

        this.cameraControl = new CameraControl(renderer, camera, () => {
            // you might want to rerender on camera update if you are not rerendering all the time
            window.requestAnimationFrame(() => renderer.render(scene, camera))
        })

        var animate = function () {
            requestAnimationFrame( animate );

            renderer.render( scene, camera );
        };

        animate();
    }

    // animate() {
    //     requestAnimationFrame( this.animate );

    //     console.log("ANIMATE");

    //     this.cube.rotation.x += 0.01;
    //     this.cube.rotation.y += 0.01;

    //     this.renderer.render( this.scene, this.camera );
    // }
}

export class gridPoint{
    X: number = 0;
    Y: number = 0;
    Z: number = 0;
    Obj: any;
}

export class CameraControl {
    zoomMode: boolean = false
    press: boolean = false
    sensitivity: number = 0.02

    constructor(renderer: THREE.Renderer, public camera: THREE.PerspectiveCamera, updateCallback:() => void){
        renderer.domElement.addEventListener('mousemove', event => {
            if(!this.press){ return }

            if(event.button == 0){
                camera.position.y -= event.movementY * this.sensitivity
                camera.position.x -= event.movementX * this.sensitivity        
            } else if(event.button == 2){
                camera.quaternion.y -= event.movementX * this.sensitivity/10
                camera.quaternion.x -= event.movementY * this.sensitivity/10
            }

            updateCallback()
        })    

        renderer.domElement.addEventListener('mousedown', () => { this.press = true })
        renderer.domElement.addEventListener('mouseup', () => { this.press = false })
        renderer.domElement.addEventListener('mouseleave', () => { this.press = false })

        document.addEventListener('keydown', event => {
            if(event.key == 'Shift'){
                this.zoomMode = true
            }
        })

        document.addEventListener('keyup', event => {
            if(event.key == 'Shift'){
                this.zoomMode = false
            }
        })

        renderer.domElement.addEventListener('wheel', (event: any) => {
            console.log("MouseWheel");
            if(this.zoomMode){ 
                camera.fov += event.wheelDelta * this.sensitivity
                camera.updateProjectionMatrix()
            } else {
                camera.position.z += event.wheelDelta * this.sensitivity
            }

            updateCallback()
        })
    }
}