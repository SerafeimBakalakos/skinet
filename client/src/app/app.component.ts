import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})

// OnInit is a lifecycle hook
export class AppComponent implements OnInit {
  title = 'Skinet';

  // Services can injected into the ctor of AppComponent
  // However it is too early to make an API request here
  constructor() {}
  
  ngOnInit(): void {
    // Best practice: make request to the API here.
    
    
  }
}
