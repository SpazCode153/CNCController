import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { CommunicationComponent } from "./communication.component";
import { ViewerCompononet } from "./viewer/viewer.component";

const routes: Routes = [{
    path: '',
    component: CommunicationComponent,
    children: [
        {
          path: 'viewer',
          component: ViewerCompononet,
        },
        {
          path: '',
          redirectTo: 'viewer',
          pathMatch: 'full',
        }
      ],
}];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
})
export class CommunicationRoutingModule
{

}