import { Component, OnInit } from "@angular/core";
import * as THREE from 'three';
import { ConvexGeometry } from "./convex-geometry";
import { FontLoader } from "./font-loader";
import { TextGeometry } from "./text-geometry";

@Component({
    selector: 'app-auto-leveling',
    templateUrl: './auto-leveling.component.html',
    styleUrls: ['./auto-leveling.component.scss']
})
export class AutoLevelingComponent implements OnInit
{
    Scene: any;
    
    gridSize: { maxY: number, maxX: number } = { maxX: 100, maxY: 100 };
    gridSegmentSize: number = 5;

    heightMapPositions: { minX: number, maxX: number, minY: number, maxY: number } 
        = { minX: 20, maxX: 60, minY: 20, maxY: 60 };
    heightMapSegmentSize: number = 5;

    //This is what would come from the CNC machine
    testHeightMapPositions: Array<{ x: number, y:number, z:number }> = [];

    cameraControl: CameraControl | undefined;

    private generateTestHeightMap() {
        for(var y = this.heightMapPositions.minY; y < this.heightMapPositions.maxY + this.heightMapSegmentSize; y=y+this.heightMapSegmentSize)
        {
            for(var x = this.heightMapPositions.minX; x < this.heightMapPositions.maxX + this.heightMapSegmentSize; x=x+this.heightMapSegmentSize)
            {
                var heightMapPos = { x: x, y:y, z:-Math.random()};
                this.testHeightMapPositions.push(heightMapPos);
                //console.log(heightMapPos);
            }
        }
    }

    private drawHeightMap() {
        var x: number = this.heightMapPositions.minX;
        var y: number = this.heightMapPositions.minY;
        var complete: boolean = false;
        while(!complete)
        {
            console.log("Find Coordinates -> ", { x, y });
            var cor1 = this.testHeightMapPositions.find(corrdinate => corrdinate.x == x && corrdinate.y == y);
            x = x + this.heightMapSegmentSize;
            var cor2 = this.testHeightMapPositions.find(corrdinate => corrdinate.x == x && corrdinate.y == y);
            y = y + this.heightMapSegmentSize;
            var cor3 = this.testHeightMapPositions.find(corrdinate => corrdinate.x == x && corrdinate.y == y);
            x = x - this.heightMapSegmentSize;
            var cor4 = this.testHeightMapPositions.find(corrdinate => corrdinate.x == x && corrdinate.y == y);

            console.log("Coordinates -> ", { cor1, cor2, cor3, cor4 });

            const geometry = new ConvexGeometry(<any>[
                new THREE.Vector3(cor1?.x, cor1?.y, cor1?.z),
                new THREE.Vector3(cor2?.x, cor2?.y, cor2?.z),
                new THREE.Vector3(cor3?.x, cor3?.y, cor3?.z),
                new THREE.Vector3(cor4?.x, cor4?.y, cor4?.z),
            ]);
            const material = new THREE.MeshBasicMaterial( { color: 0x00ff00 } );
            const mesh = new THREE.Mesh( geometry, material );
            this.Scene.add( mesh );

            if(y == this.heightMapPositions.maxY)
            {
                if(x == (this.heightMapPositions.maxX - this.heightMapSegmentSize))
                {
                    complete = true;
                }
                else
                {
                    y = this.heightMapPositions.minY;
                    x = x + this.gridSegmentSize;
                }
            }
        }
    }

    ngOnInit(): void {
        this.generateTestHeightMap();

        var scene = new THREE.Scene();
        var camera = new THREE.PerspectiveCamera( 75, window.innerWidth/window.innerHeight, 0.1, 1000 );
        //camera.position.set(0,2.5,2.5);
        //camera.lookAt(new THREE.Vector3(0,0,0));

        var renderer = new THREE.WebGLRenderer();
        renderer.setClearColor( 0xffffff, 0);
        renderer.setSize( window.innerWidth - 25, window.innerHeight - 100 );
        document.body.appendChild( renderer.domElement );

        const lineMaterial = new THREE.LineBasicMaterial( { color: 0x000000 } );

        const yPoints = [];
        for(var x = 0; x < this.gridSize.maxX + this.gridSegmentSize; x=x+this.gridSegmentSize)
        {
            yPoints.push( new THREE.Vector3( x, 0, 0 ) );
            yPoints.push( new THREE.Vector3( x, this.gridSize.maxY, 0 ) );
            yPoints.push( new THREE.Vector3( x, 0, 0 ) );
        }

        const yGeometry = new THREE.BufferGeometry().setFromPoints( yPoints );
        const yline = new THREE.Line( yGeometry, lineMaterial );
        scene.add(yline);

        const xPoints = [];
        for(var y = 0; y < this.gridSize.maxY + this.gridSegmentSize; y=y+this.gridSegmentSize)
        {
            xPoints.push( new THREE.Vector3( 0, y, 0 ) );
            xPoints.push( new THREE.Vector3( this.gridSize.maxX, y, 0 ) );
            xPoints.push( new THREE.Vector3( 0, y, 0 ) );
        }

        const xGeometry = new THREE.BufferGeometry().setFromPoints( xPoints );
        const xline = new THREE.Line( xGeometry, lineMaterial );
        scene.add(xline);

        const loader = new FontLoader(undefined);
        loader.load('assets/fonts/Audiowide_Regular.json', function ( font: any ) {
            console.log("Font Loaded!");
            const geometry = new TextGeometry( 'X,Y(0,0)', {
                font: font,
                size: 60,
                height: 5,
                curveSegments: 24,
                bevelEnabled: false,
                bevelThickness: 5,
                bevelSize: 4,
                bevelOffset: 0,
                bevelSegments: 10
            } );

            var material = new THREE.MeshLambertMaterial({
                color: 0xF3FFE2
              });
            var mesh = new THREE.Mesh(geometry, material);
            mesh.position.set(-4, 0, 0);
            mesh.scale.multiplyScalar(0.01)
            mesh.castShadow = true;

            scene.add(mesh)
        }, undefined, (err: any) => { console.log("Font Load Failed!!!", err); });

        camera.position.z = 50;

        this.cameraControl = new CameraControl(renderer, camera, () => {
            // you might want to rerender on camera update if you are not rerendering all the time
            window.requestAnimationFrame(() => renderer.render(scene, camera))
        })

        var animate = function () {
            requestAnimationFrame( animate );

            renderer.render( scene, camera );
        };

        this.Scene = scene;

        animate();

        this.drawHeightMap();
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
    shiftKey: boolean = false
    button1Press: boolean = false;
    button2Press: boolean = false;
    sensitivity: number = 0.07;
    rotateSensitivity: number = 0.02;
    zoomSensitivity: number = 0.02;

    constructor(renderer: THREE.Renderer, public camera: THREE.PerspectiveCamera, updateCallback:() => void){
        renderer.domElement.addEventListener('mousemove', event => {
            if(!this.button1Press && !this.button2Press){ return }

            if(this.button1Press){
                camera.position.y += event.movementY * this.sensitivity
                camera.position.x -= event.movementX * this.sensitivity        
            } else if(this.button2Press && !this.shiftKey){
                camera.quaternion.y -= event.movementX * this.rotateSensitivity/10
                camera.quaternion.x -= event.movementY * this.rotateSensitivity/10
            } else if(this.button2Press && this.shiftKey){
                camera.quaternion.z -= event.movementY * this.rotateSensitivity/10
            }

            updateCallback()
        })    

        renderer.domElement.addEventListener('mousedown', (event) => 
        { 
            if(event.buttons == 1)
            {
                this.button1Press = true;
            }
            if(event.buttons == 2)
            {
                this.button2Press = true;
            }
        });
        renderer.domElement.addEventListener('mouseup', (event) => 
        { 
            this.button1Press = false;
            this.button2Press = false;
        });
        renderer.domElement.addEventListener('mouseleave', () => 
        { 
            this.button1Press = false;
            this.button2Press = false; 
        });
        renderer.domElement.addEventListener('contextmenu', (event) => 
        { 
            event.preventDefault();
        });

        document.addEventListener('keydown', event => {
            if(event.key == 'Shift'){
                this.shiftKey = true
            }
        })

        document.addEventListener('keyup', event => {
            if(event.key == 'Shift'){
                this.shiftKey = false
            }
        })

        renderer.domElement.addEventListener('wheel', (event: any) => {
            if(this.shiftKey){ 
                camera.fov += event.wheelDelta * this.zoomSensitivity
                camera.updateProjectionMatrix()
            } else {
                camera.position.z -= event.wheelDelta * this.zoomSensitivity;

                //TODO: Determine the cameras direction and move towards that
                //camera.position.x -= event.wheelDelta * this.zoomSensitivity;
                //camera.position.y -= event.wheelDelta * this.zoomSensitivity;
            }

            updateCallback()
        })
    }
}