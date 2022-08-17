import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { NotFoundComponent } from "./not-found/not-found.compionent";
import { PagesComponent } from "./pages.component";

const routes: Routes = [{
    path: '',
    component: PagesComponent,
    children: [
        {
            path: 'auto-leveling',
            loadChildren: () => import('./auto-leveling/auto-leveling.module')
                .then(m => m.AutoLevelingModule)
        },
        {
            path: 'communication',
            loadChildren: () => import('./communication/communication.module')
                .then(m => m.CommunicationModule)
        },
        {
            path: '',
            redirectTo: 'communication',
            pathMatch: 'full',
        },
        {
            path: '**',
            component: NotFoundComponent,
        }
    ],
  }];

@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule],
  })
  export class PagesRoutingModule {
  }