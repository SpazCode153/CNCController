import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { PagesRoutingModule } from "./pages-routing.module";
import { PagesComponent } from "./pages.component";

@NgModule({
    imports: [
      CommonModule,
      PagesRoutingModule
    ],
    declarations: [
      PagesComponent
    ],
  })
  export class PagesModule {
  }