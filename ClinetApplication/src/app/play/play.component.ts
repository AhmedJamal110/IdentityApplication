import { Component, OnInit } from '@angular/core';
import { PlayService } from './play.service';



@Component({
  selector: 'app-play',
  templateUrl: './play.component.html',
  styleUrls: ['./play.component.css']
})
export class PlayComponent implements OnInit {


  messages : string | undefined;
  constructor(private _PlayService:PlayService ){}


  ngOnInit(): void {
    this.Players();
  }




  Players(){
    this._PlayService.getPlayers().subscribe({
      next:(response : any) =>  this.messages = response.value.message,
      error:(err) => console.log(err),
  })


  } 

}
