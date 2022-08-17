import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { NgbModule } from "@ng-bootstrap/ng-bootstrap";
import { AutoLevelingRoutingModule } from "./auto-leveling-routing.module";
import { AutoLevelingComponent } from "./auto-leveling.component";

@NgModule({
    imports: [
        AutoLevelingRoutingModule,
        CommonModule,
        NgbModule,
    ],
    declarations: [
        AutoLevelingComponent
    ]
})
export class AutoLevelingModule {

}