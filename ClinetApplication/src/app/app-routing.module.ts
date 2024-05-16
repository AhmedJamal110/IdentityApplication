import { AccountModule } from './account/account.module';
import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeComponent } from './home/home.component';
import { NotfoundComponent } from './shared/components/errors/notfound/notfound.component';
import { PlayComponent } from './play/play.component';
import { AuthorizationGuard } from './shared/guards/authorization.guard';

const routes: Routes = [
  {path: '' , component : HomeComponent},
  
  {path: '' ,
   runGuardsAndResolvers: 'always',
    canActivate : [AuthorizationGuard],
    children : [ {path : 'play' , component : PlayComponent}]
  
  },

  {path:"account", loadChildren:()=> import("./account/account.module").then(module=>module.AccountModule) },
  

{path: "not-found" , component : NotfoundComponent},
{path: "**" , component : NotfoundComponent},


];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
