import { Component, OnInit, ViewChild } from "@angular/core";
import * as signalR from '@microsoft/signalr';
import { signalRRetryPolicy } from "src/app/shared/signalr-retry-policy";

@Component({
    selector: 'app-communication-viewer',
    templateUrl: './viewer.component.html',
    styleUrls:['./viewer.component.scss']
})
export class ViewerCompononet implements OnInit
{
    @ViewChild("commData") commData: any;
    active = 1;
    data = "";
    input = "";

    comPort = "";
    baudRate = 9600;
    databits = 8;
    stopbits = 1;
    partity = 0;

    connection: any;

    sendMessage() {
        this.data = this.data + this.input + "\n";

        this.connection.invoke("SendMessage", this.input).then(() => 
        {
            if(this.commData != undefined)
            {
                this.commData.nativeElement.scrollTop = this.commData.nativeElement.scrollHeight;
            }
        });
    }

    setCommunicationSettings()
    {
        this.connection.invoke("Set", this.comPort, this.baudRate, this.partity, this.databits, this.stopbits).then(() => {});
    }

    setComPort(event: any)
    {
        this.comPort = event.srcElement.value;
    }

    setBaudRate(event: any)
    {
        this.baudRate = +event.srcElement.value;
    }

    setDatabits(event: any)
    {
        this.databits = +event.srcElement.value;
    }

    setStopbits(event: any)
    {
        this.stopbits = +event.srcElement.value;
    }

    setParity(event: any)
    {
        this.partity = +event.srcElement.value;
    }


    setInput(event: any)
    {
        this.input = event.srcElement.value;
    }

    ngOnInit(): void {
        this.connection = new signalR.HubConnectionBuilder()
                .withUrl("http://localhost:5020/communicationhub")
                .withAutomaticReconnect(new signalRRetryPolicy())
                .build();

        this.start();
    }

    async start() {
        try {
            await this.connection.start();
            this.connection.on("newSerialMessage", (message: string) => {
                this.data = this.data + message;
                console.log(this.commData);
                if(this.commData != undefined)
                {
                    this.commData.nativeElement.scrollTop = this.commData.nativeElement.scrollHeight;
                }
            });
        } catch (err) {
            
        }
    }

    connect()
    {
        this.connection.invoke("Connect").then(() => {});
    }

    disconnect()
    {
        this.connection.invoke("Disconnect").then(() => {});
    }
}