import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { CommunicationRoutingModule } from "./communication-routing.module";
import { CommunicationComponent } from "./communication.component";
import { ViewerCompononet } from "./viewer/viewer.component";

@NgModule({
    imports: [
        CommunicationRoutingModule,
        CommonModule,
        NgbModule,
    ],
    declarations: [
        CommunicationComponent,
        ViewerCompononet
    ]
})
export class CommunicationModule {

}