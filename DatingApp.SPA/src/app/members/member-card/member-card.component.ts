import { Component, OnInit, Input } from '@angular/core';
import { User } from '../../_models/User';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {
  // bringing the user data from the parent component to the child component
  // i.e the member-list component
  // hence need to the the Input property
  @Input() user: User;
  constructor() { }

  ngOnInit() {
  }

}
