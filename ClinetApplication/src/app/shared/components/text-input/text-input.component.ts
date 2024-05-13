import { Component, Input, Self } from '@angular/core';
import { ControlValueAccessor, FormControl, NgControl } from '@angular/forms';


@Component({
  selector: 'app-text-input',
  templateUrl: './text-input.component.html',
  styleUrls: ['./text-input.component.css']
})
export class TextInputComponent implements ControlValueAccessor {

  @Input() type = 'text';
  @Input() label = '';

  constructor(@Self() public _NgControl:NgControl  ){
    this._NgControl.valueAccessor= this;
  }

  writeValue(obj: any): void {
  }
  registerOnChange(fn: any): void {
  }
  registerOnTouched(fn: any): void {
  }

   get control() : FormControl{
    return this._NgControl.control as FormControl
   }

}
