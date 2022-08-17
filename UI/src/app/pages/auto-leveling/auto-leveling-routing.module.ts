import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AutoLevelingComponent } from "./auto-leveling.component";

const routes: Routes = [{
    path: '',
    component: AutoLevelingComponent,
    children: [
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
export class AutoLevelingRoutingModule
{

}