import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AuthService } from '../_services/auth.service';
import { AlertifyService } from '../_services/alertify.service';


@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  model: any = {};
  // @Input() valuesFromHome: any; // to get value from that parent component
  @Output() cancelRegister = new EventEmitter(); // to pass event from child to parent
  constructor(private authService: AuthService, private alertyfy: AlertifyService) { }

  ngOnInit() {
  }

  register() {
    console.log(this.model);
    this.authService.register(this.model).subscribe(() => {
      this.alertyfy.success('Registration Successful');
    }, error => {
      this.alertyfy.error(error);
    });
  }

  cancel() {
    this.cancelRegister.emit(false);
  }

}
