import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { environment } from 'src/environments/environment'; //careful: not environment.prod.ts

@Component({
  selector: 'app-test-error',
  templateUrl: './test-error.component.html',
  styleUrls: ['./test-error.component.scss']
})
export class TestErrorComponent {
  baseUrl = environment.apiUrl;

  constructor(private http: HttpClient) {}

  get404Error() {
    // We use "products/42", because we know it does not exist
    this.http.get(this.baseUrl + 'products/42').subscribe({
      next: response => console.log(response), // This should not be hit by the test, but must exist
      error: err => console.log(err)
    });
  }

  get500Error() {
    // "buggy/servererror" must match the endpoint we created in the ErrorController of the API
    this.http.get(this.baseUrl + 'buggy/servererror').subscribe({
      next: response => console.log(response), 
      error: err => console.log(err)
    });
  }

  get400Error() {
    this.http.get(this.baseUrl + 'buggy/badrequest').subscribe({
      next: response => console.log(response),
      error: err => console.log(err)
    });
  }

  get400ValidationError() {
    this.http.get(this.baseUrl + 'products/fortytwo').subscribe({
      next: response => console.log(response),
      error: err => console.log(err)
    });
  }
}
